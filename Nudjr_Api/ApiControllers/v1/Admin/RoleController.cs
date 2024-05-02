using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nudjr_AppCore.Services.IdentityServices.Interfaces;
using Nudjr_Domain.Enums;
using Nudjr_Domain.Models;
using Nudjr_Domain.Models.IdentityModels;
using Nudjr_Domain.Models.ResposneModels;
using Nudjr_Domain.Models.ViewModels;
using System.Net;

namespace Nudjr_Api.ApiControllers.v1.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Admin/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class RoleController : BaseController
    {
        private readonly IIdentityUserService _identityUserService;
        private readonly IUserRoleService _roleService;
        public RoleController(IIdentityUserService identityUserService, IUserRoleService userRoleService)
        {
            _identityUserService = identityUserService;
            _roleService = userRoleService;
        }

        /// <summary>
        /// Assign Role To User
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Assign")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AssignRole(AdminRoleRequestModel model)
        {
            ApplicationUser user = await _identityUserService.GetApplicationUser(User);
            Enum.TryParse<PersonType>(model.Role, out PersonType personType);
            bool isSuccess = await _roleService.AssignUserToRole(user, user.Id, personType);
            string message = isSuccess ? "Role Updated Successfully" : "Role Update Failed";
            return Ok(isSuccess, message);
        }


        /// <summary>
        /// Remove User From Role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Remove")]
        [ProducesResponseType(typeof(ApiResponseModel<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveUserFromRole(AdminRoleRequestModel model)
        {
            ApplicationUser user = await _identityUserService.GetApplicationUser(User);
            Enum.TryParse<PersonType>(model.Role, out PersonType personType);
            RemoveRoleResponseModel response = await _roleService.RemoveUserFromRole(user, user.Id, personType);
            string message = response.IsSuccess ? "Role Removed Successfully" : "Failed To Remove User From Role";
            return Ok(response.IsSuccess, message);
        }
    }
}
