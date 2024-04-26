using Nudjr_Domain.Models.ServiceModels.NovuModels;

namespace Nudjr_AppCore.Services.Shared.Interfaces
{
    public interface INovuNotificationService
    {
        Task<bool> AddSubscribersToTopic(string topicKey);
        Task<bool> CreateATopic(string key, string name);
        Task<NovuOperationModel> CreateSubscriber(Guid userId);
        Task<bool> SendNotificationToAll(string eventName, object payload);
        Task<bool> SendNotificationToASubscriber(string subscriberId, string eventName, object payload);
        Task<bool> UpdateSubscriber(Guid userId, UpdateSubscriberDTO model);
    }
}
