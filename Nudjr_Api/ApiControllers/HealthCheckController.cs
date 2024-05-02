using Microsoft.AspNetCore.Mvc;

namespace Nudjr_Api.ApiControllers
{
    [Route("api/healthcheck")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        public HealthCheckController()
        {

        }

        /// <summary>
        /// Check Liveness
        /// </summary>
        /// <returns></returns>
        [HttpGet("liveness")]
        public IActionResult Liveness()
        {
            return Ok();
        }

        /// <summary>
        /// Check Readiness
        /// </summary>
        /// <returns></returns>
        [HttpGet("readiness")]
        public async Task<IActionResult> Readiness()
        {
            return Ok();
        }
    }
}
