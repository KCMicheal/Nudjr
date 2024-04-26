using Nudjr_Domain.Enums;
using Nudjr_Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Nudjr_Api.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public override OkObjectResult Ok(object value)
        {
            return base.Ok(new ApiResponseModel<Object>
            {
                Status = ResponseStatus.OK,
                Message = "",
                Data = value
            });
        }

        [NonAction]
        public OkObjectResult OkBase(object value)
        {
            return base.Ok(value);
        }


        [NonAction]
        public OkObjectResult Ok(object value, string message = "", ResponseStatus status = ResponseStatus.OK)
        {
            return base.Ok(new ApiResponseModel<object>
            {
                Status = status,
                Message = message,
                Data = value
            });
        }

        [NonAction]

        public override BadRequestObjectResult BadRequest(object value)
        {
            return base.BadRequest(new ApiResponseModel<object>
            {
                Status = ResponseStatus.APP_ERROR,
                Message = "Successful Transaction",
                Data = value
            });
        }

        [NonAction]

        public BadRequestObjectResult BadRequest(object value, string message = "", ResponseStatus status = ResponseStatus.FATAL_ERROR)
        {
            return base.BadRequest(new ApiResponseModel<object>
            {
                Status = status,
                Message = message,
                Data = value
            });
        }
    }
}
