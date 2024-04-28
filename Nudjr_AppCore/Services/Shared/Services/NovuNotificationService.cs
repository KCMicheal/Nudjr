using AutoMapper;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models.ConfigModels;
using Nudjr_Domain.Models.ServiceModels.NovuModels;
using Nudjr_Persistence.UnitOfWork.Interfaces;
using Microsoft.Extensions.Options;
using Novu;
using Novu.DTO.Events;
using Novu.DTO.Subscribers;
using Novu.DTO.Topics;
using Novu.Models;

namespace Nudjr_AppCore.Services.Shared.Services
{
    public class NovuNotificationService : BaseEntityService, INovuNotificationService
    {
        private readonly NovuConfig _novuConfig;
        public NovuNotificationService(IUnitOfWork unitOfWork, IMapper mapper,
            IOptionsSnapshot<NovuConfig> novuConfig) : base(unitOfWork, mapper)
        {
            _novuConfig = novuConfig.Value;
        }

        public async Task<NovuOperationModel> CreateSubscriber(Guid userId)
        {
            try
            {
                var novuConfiguration = new NovuClientConfiguration { ApiKey = _novuConfig.ApiKey };
                var novuClient = new NovuClient(novuConfiguration);

                USER user = await _unitOfWork.GetRepository<USER>().SingleOrDefaultAsync(x => x.Id == userId && x.EntityStatus == EntityStatus.ACTIVE);
                if (user is null)
                    throw new InvalidOperationException("User Not Found");

                NOVUSUBSCRIBER subscribedUser = await _unitOfWork.GetRepository<NOVUSUBSCRIBER>().FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (subscribedUser is not null)
                    throw new InvalidOperationException("User Already Subscribed");

                var newSubscriber = new SubscriberCreateData()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.EmailAddress,
                    Avatar = user.ImageURL,
                    SubscriberId = await GenerateSubscriberID(),
                };

                var createdSubscriber = await novuClient.Subscriber.Create(newSubscriber);
                if (createdSubscriber != null)
                {
                    NOVUSUBSCRIBER subscriber = new NOVUSUBSCRIBER()
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        EntityStatus = EntityStatus.ACTIVE,
                        SubscriberId = newSubscriber.SubscriberId,
                        UserId = user.Id,
                    };

                    await _unitOfWork.GetRepository<NOVUSUBSCRIBER>().AddAsync(subscriber);

                    await _unitOfWork.SaveChangesAsync();

                    return new NovuOperationModel
                    {
                        Success = true,
                        Result = newSubscriber.SubscriberId
                    };
                }

                return new NovuOperationModel { Success = false };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> UpdateSubscriber(Guid userId, UpdateSubscriberDTO model)
        {
            try
            {
                var novuConfiguration = new NovuClientConfiguration { ApiKey = _novuConfig.ApiKey };
                var novuClient = new NovuClient(novuConfiguration);

                USER user = await _unitOfWork.GetRepository<USER>().SingleOrDefaultAsync(x => x.Id == userId && x.EntityStatus == EntityStatus.ACTIVE);
                if (user is null)
                    throw new InvalidOperationException("User Not Found");

                NOVUSUBSCRIBER subscribedUser = await _unitOfWork.GetRepository<NOVUSUBSCRIBER>().FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (subscribedUser is null)
                    throw new InvalidOperationException("User Not Subscribed Yet");

                SubscriberEditData subscriberEditData = new SubscriberEditData()
                {
                    Email = model.Email ?? user.EmailAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };

                var updatedSubscriber = await novuClient.Subscriber.Update(subscribedUser.SubscriberId, subscriberEditData);
                if (updatedSubscriber is null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }


        public async Task<bool> SendNotificationToASubscriber(string subscriberId, string eventName, object payload)
        {
            try
            {
                var novuConfiguration = new NovuClientConfiguration { ApiKey = _novuConfig.ApiKey };
                var novuClient = new NovuClient(novuConfiguration);

                var payloadNotification = new EventCreateData
                {
                    EventName = eventName,
                    To = { SubscriberId = subscriberId },
                    Payload = payload
                };

                var trigger = await novuClient.Event.Trigger(payloadNotification);

                if (trigger.Data.Acknowledged)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> SendNotificationToAll(string eventName, object payload)
        {
            try
            {
                var novuConfiguration = new NovuClientConfiguration { ApiKey = _novuConfig.ApiKey };
                var novuClient = new NovuClient(novuConfiguration);

                var payloadTopic = new BroadcastEventCreateData()
                {
                    Name = eventName,
                    Payload = payload
                };

                var triggerTopic = await novuClient.Event.CreateBroadcast(payloadTopic);

                if (triggerTopic.Data.Acknowledged)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<bool> CreateATopic(string key, string name)
        {
            var novuConfiguration = new NovuClientConfiguration { ApiKey = _novuConfig.ApiKey };
            var novuClient = new NovuClient(novuConfiguration);

            var topicRequest = new Novu.DTO.Topics.TopicCreateData()
            {
                Key = key,
                Name = name,
            };

            var createdTopic = await novuClient.Topic.Create(topicRequest);
            if (createdTopic != null)
                return true;

            return false;
        }

        public async Task<bool> AddSubscribersToTopic(string topicKey)
        {
            try
            {
                var novuConfiguration = new NovuClientConfiguration { ApiKey = _novuConfig.ApiKey };
                var novuClient = new NovuClient(novuConfiguration);

                var topic = await novuClient.Topic.Get(topicKey);
                if (topic == null)
                    throw new InvalidOperationException("Topic Does Not Exist!");

                IEnumerable<NOVUSUBSCRIBER> subscribedUsers = await _unitOfWork.GetRepository<NOVUSUBSCRIBER>().GetListAsync();

                List<string> users = new List<string>();
                foreach (var user in subscribedUsers)
                {
                    users.Add(user.SubscriberId);
                }
                var subscriberList = new TopicSubscriberCreateData(users);

                var result = await novuClient.Topic.AddSubscriber(topicKey, subscriberList);
                if (result is not null)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        private async Task<string> GenerateSubscriberID()
        {
            string prefix = "DT_NS_";
            int roleNumber = 00001;
            var prefixID = $"{prefix}{roleNumber}";
            NOVUSUBSCRIBER subscriber = await _unitOfWork.GetRepository<NOVUSUBSCRIBER>()
                .LastOrDefaultAsync(orderBy: x => x.OrderByDescending(y => y.CreatedAt));

            if (subscriber is not null)
            {
                string? roleId = subscriber.SubscriberId.Split("DT_NS_").LastOrDefault();
                int roleNo = int.Parse(roleId);
                prefixID = $"{prefix}{roleNo + 1}";
            }
            else
            {
                return prefixID;
            }

            return prefixID;
        }
    }
}
