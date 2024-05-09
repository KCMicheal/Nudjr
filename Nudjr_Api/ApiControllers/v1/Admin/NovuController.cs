using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Models;
using Nudjr_Domain.Models.ResposneModels;
using Nudjr_Domain.Models.ServiceModels.NovuModels;
using System.Net;

namespace Nudjr_Api.ApiControllers.v1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Admin/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class NovuController : BaseController
    {
        private readonly INovuNotificationService _notificationService;
        public NovuController(INovuNotificationService novuNotificationService)
        {
            _notificationService = novuNotificationService;
        }


        /// <summary>
        /// Creates A Subscriber On Novu
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("Subscriber/Create")]
        [ProducesResponseType(typeof(ApiResponseModel<NovuOperationModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateASubscriber([FromQuery] Guid userId)
        {
            NovuOperationModel response = await _notificationService.CreateSubscriber(userId);
            string isSuccess = response.Success ? "Subscriber Created Successfully" : "Error Occurred Creating Subscriber";
            return Ok(response, isSuccess);
        }


        /// <summary>
        /// Updates A Subscriber On Novu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Subscriber/Update")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateASubscriber([FromQuery] Guid userId, [FromBody] UpdateSubscriberDTO model)
        {
            var response = await _notificationService.UpdateSubscriber(userId, model);
            string isSuccess = response ? "Subscriber Updated Successfully" : "Error Occurred Updating Subscriber";
            return Ok(response, isSuccess);
        }


        /// <summary>
        /// Sends A Notification To A Subscriber
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <param name="eventName"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost("Subscriber/Send")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> NotifyASubscriber([FromQuery] string subscriberId, string eventName, WelcomeMessage payload)
        {
            var response = await _notificationService.SendNotificationToASubscriber(subscriberId, eventName, payload);
            string isSuccess = response ? "Notification Sent Successfully To Subscriber" : "Error Sending Notification To Subscriber";
            return Ok(response, isSuccess);
        }


        /// <summary>
        /// Sends A Notification To All Subscribers
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost("Subscriber/SendToAll")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> NotifyAllSubscribersInATopic([FromQuery] string eventName, object payload)
        {
            var response = await _notificationService.SendNotificationToAll(eventName, payload);
            string isSuccess = response ? "Notification Sent Successfully To Subscribers" : "Error Sending Notification To Subscribers";
            return Ok(response, isSuccess);
        }


        /// <summary>
        /// Creates A Topic
        /// </summary>
        /// <param name="topicKey"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("Subscriber/Topic/Create")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatesATopic([FromQuery] string topicKey, string name)
        {
            var response = await _notificationService.CreateATopic(topicKey, name);
            string isSuccess = response ? "Notification Topic Created Successfully" : "Error Creating A Topic";
            return Ok(response, isSuccess);
        }


        /// <summary>
        /// Adds Subscribers To A Topic
        /// </summary>
        /// <param name="topicKey"></param>
        /// <returns></returns>
        [HttpPost("Subscriber/Topic/AddSubscribers")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddASubscriberToATopic([FromQuery] string topicKey)
        {
            var response = await _notificationService.AddSubscribersToTopic(topicKey);
            string isSuccess = response ? "Added Subscribers To Topic Successfully" : "Error Adding Subscribers To A Topic";
            return Ok(response, isSuccess);
        }
    }
}
