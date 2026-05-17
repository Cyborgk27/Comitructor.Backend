using Comitructor.Application.Dtos.Request;
using Comitructor.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comitructor.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _requestService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _requestService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto input)
        {
            var result = await _requestService.CreateAsync(input);
            return Ok(result);
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus(
            [FromQuery] int requestId,
            [FromQuery] string newStatus,
            [FromQuery] string reason)
        {
            await _requestService.UpdateStatusAsync(requestId, newStatus, reason);
            return NoContent();
        }

        [HttpPut("assign")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Assign(
            [FromQuery] int requestId,
            [FromQuery] int userId)
        {
            await _requestService.AssignRequestAsync(requestId, userId);
            return Ok(new { message = "Solicitud asignada correctamente" });
        }
    }
}
