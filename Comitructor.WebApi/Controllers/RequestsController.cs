using Comitructor.Application.Dtos.Request;
using Comitructor.Application.Interfaces;
using Comitructor.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comitructor.WebApi.Controllers
{
    /// <summary>
    /// Controlador para la gestión de solicitudes de mantenimiento.
    /// Requiere autenticación mediante JWT para todos sus endpoints.
    /// </summary>
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

        /// <summary>
        /// Obtiene la lista de solicitudes.
        /// </summary>
        /// <returns>Lista de solicitudes filtradas por rol.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var result = await _requestService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el detalle de una solicitud específica por su ID.
        /// </summary>
        /// <param name="id">ID numérico de la solicitud.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<RequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _requestService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Registra una nueva solicitud de mantenimiento en el sistema.
        /// </summary>
        /// <param name="input">Datos básicos de la solicitud.</param>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto input)
        {
            var result = await _requestService.CreateAsync(input);
            return Ok(result);
        }

        /// <summary>
        /// Actualiza una solicitud de mantenimiento en el sistema.
        /// </summary>
        /// <param name="input">Datos básicos de la solicitud.</param>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UpdateRequestDto input)
        {
            var result = await _requestService.UpdateAsync(input);
            return Ok(result);
        }

        /// <summary>
        /// Actualiza el estado de una solicitud y registra el motivo en el historial.
        /// </summary>
        [HttpPut("update-status")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(
            [FromQuery] int requestId,
            [FromQuery] string newStatus,
            [FromQuery] string reason)
        {
            await _requestService.UpdateStatusAsync(requestId, newStatus, reason);
            return NoContent();
        }

        /// <summary>
        /// Asigna una solicitud a un operador específico. 
        /// </summary>
        [HttpPut("assign")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Assign(
            [FromQuery] int requestId,
            [FromQuery] int userId)
        {
            await _requestService.AssignRequestAsync(requestId, userId);
            return Ok(new { message = "Solicitud asignada correctamente" });
        }

        /// <summary>
        /// Obtiene una lista de usuarios simplificada para componentes de selección.
        /// </summary>
        /// <returns>Lista de usuarios con ID y nombre.</returns>
        /// <response code="200">Retorna la lista de usuarios activos.</response>
        /// <response code="401">Si el usuario no está autenticado.</response>
        [HttpGet("users-lookup")]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersLookup()
        {
            var users = await _requestService.GetUsersForSelectAsync();
            return Ok(users);
        }
    }
}