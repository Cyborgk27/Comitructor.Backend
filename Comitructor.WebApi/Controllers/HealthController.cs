using Comitructor.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comitructor.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Verifica el estado de salud del servicio.
        /// </summary>
        /// <returns>Estado operativo del API.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Service is healthy and running",
                Data = "Healthy",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
