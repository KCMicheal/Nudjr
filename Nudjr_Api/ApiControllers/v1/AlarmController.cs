using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_AppCore.Services.Shared.Interfaces;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Models;
using Nudjr_Domain.Models.Dtos;
using Nudjr_Domain.Models.ResposneModels;
using Nudjr_Domain.Models.ServiceModels;
using System.Net;

namespace Nudjr_Api.ApiControllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class AlarmController : BaseController
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IAlarmService _alarmService;
        public AlarmController(IUserAccountService userAccountService, IAlarmService alarmService)
        {
            _userAccountService = userAccountService;
            _alarmService = alarmService;
        }


        /// <summary>
        /// Creates Alarm For Logged In User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponseModel<ServiceOperationModel<AlarmDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAlarm([FromBody] CreateAlarmDto model)
        {
            USER user = await _userAccountService.GetUser(User);
            var response = await _alarmService.CreateAlarm(model, user);

            return Ok(response);
        }
    }
}
