using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_Domain.Entities;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models;
using Nudjr_Domain.Models.ResposneModels;
using Nudjr_Domain.Models.ServiceModels;
using Nudjr_Domain.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Asp.Versioning;

namespace Nudjr_Api.ApiControllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : BaseController
    {
        private readonly IUserAccountService _userAccountService;
        public AuthController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }


        /// <summary>
        /// Creates New User and Returns Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponseModel<Jwt>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateUserAccount([FromBody] UserSignUpDto model)
        {
            USER tribeUser = await _userAccountService.CreateUserAccount(model, model.Username, model.Password);
            Jwt tokenResponse = await _userAccountService.GenerateLoginTokenAfterAccountCreation(tribeUser);
            return Ok(tokenResponse);
        }



        /// <summary>
        /// Logs In User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponseModel<JwtWithRefreshToken>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LogInUserAccount([FromBody] UserSignInDto model)
        {
            JwtWithRefreshToken payload = await _userAccountService.UserLogin(model.UserNameOrEmail, model.Password, model.PersonType);
            return Ok(payload, "Login Successful", ResponseStatus.OK);
        }


        /// <summary>
        /// Creates New Admin Or Super Admin and Returns Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost("Admin/Create")]
        [ProducesResponseType(typeof(ApiResponseModel<Jwt>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAdminAccount([FromBody] UserSignUpDto model)
        {
            USER tribeUser = await _userAccountService.CreateAdminAccount(model);
            Jwt tokenResponse = await _userAccountService.GenerateLoginTokenAfterAccountCreation(tribeUser);
            return Ok(tokenResponse);
        }

        /// <summary>
        /// Logs In Admin Or SuperAdmin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Admin/Login")]
        [ProducesResponseType(typeof(ApiResponseModel<Jwt>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LogInAdminAccount([FromBody] AdminSignInDto model)
        {
            Jwt payload = await _userAccountService.AdminLogin(model.UserNameOrEmailAddress, model.Password);
            return Ok(payload, "Login Successful", ResponseStatus.OK);
        }


        /// <summary>
        /// Renew Authorization Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("RenewJWT")]
        [ProducesResponseType(typeof(ApiResponseModel<JwtWithRefreshToken>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RenewJWT([FromBody] RenewJWTTokenModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException("Input Parameters Not In Correct Format");
            }

            JwtWithRefreshToken payload = await _userAccountService.RefreshToken(model.token, model.refreshToken);
            return Ok(payload, "JWt Payload Generated Successfully", ResponseStatus.OK);

        }
    }
}
