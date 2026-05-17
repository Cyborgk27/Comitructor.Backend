using Comitructor.Application.Dtos.Request;
using Comitructor.Application.Interfaces;
using Comitructor.Domain.Entities;
using Comitructor.Domain.Enums;
using Comitructor.Domain.Exceptions;
using Comitructor.Domain.Interfaces;
using Comitructor.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Comitructor.Application.Services
{
    /// <summary>
    /// Servicio de aplicación para la gestión del ciclo de vida de las solicitudes de mantenimiento.
    /// Implementa reglas de negocio para asignación, cambio de estados y auditoría.
    /// </summary>
    public class RequestService : IRequestService
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RequestService> _logger;

        public RequestService(
            ICurrentUserProvider currentUserProvider,
            ApplicationDbContext context,
            ILogger<RequestService> logger)
        {
            _currentUserProvider = currentUserProvider;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva solicitud generando un código correlativo y asignando auditoría inicial.
        /// </summary>
        /// <param name="dto">Datos de la solicitud enviados desde el cliente.</param>
        /// <returns>ID de la solicitud generada.</returns>
        public async Task<int> CreateAsync(CreateRequestDto dto)
        {
            _logger.LogInformation("Creando nueva solicitud: {Title}", dto.Title);

            // Generación de código de negocio (SOL-XXX)
            var count = await _context.Requests.CountAsync() + 1;
            var code = $"REQ-{count:D3}";

            var request = new Request
            {
                Code = code,
                Title = dto.Title,
                Description = dto.Description,
                Priority = Enum.Parse<RequestPriority>(dto.Priority),
                Area = Enum.Parse<RequestArea>(dto.Area),
                Status = RequestStatus.New,
                CreatedBy = _currentUserProvider.UserId,
                CreatedDate = DateTime.UtcNow,
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return request.Id;
        }

        /// <summary>
        /// Obtiene todas las solicitudes aplicando filtros de seguridad basados en el rol del usuario.
        /// </summary>
        /// <remarks>
        /// Los Administradores visualizan todo el universo de datos. 
        /// Los Operadores están limitados únicamente a las solicitudes asignadas a su identificador.
        /// </remarks>
        /// <returns>Colección de DTOs de solicitud.</returns>
        public async Task<IEnumerable<RequestDto>> GetAllAsync()
        {
            var query = _context.Requests
                .Include(r => r.AssignedUser)
                .AsQueryable();

            // Lógica de seguridad: Filtrado por pertenencia de datos
            if (_currentUserProvider.Role == UserRole.Operator.ToString())
            {
                query = query.Where(r => r.AssignedUserId == _currentUserProvider.UserId);
            }

            return await query
                .Select(r => new RequestDto(
                    r.Id,
                    r.Code,
                    r.Title,
                    r.Description,
                    r.Status.ToString(),
                    r.Priority.ToString(),
                    r.Area.ToString(),
                    r.AssignedUser != null ? r.AssignedUser.Username : "Sin asignar",
                    r.CreatedDate ?? DateTime.Now
                ))
                .ToListAsync();
        }

        /// <summary>
        /// Realiza la transición de estado de una solicitud y registra el rastro en el historial.
        /// </summary>
        /// <param name="requestId">Identificador único de la solicitud.</param>
        /// <param name="newStatus">Estado destino de la transición.</param>
        /// <param name="reason">Explicación del cambio para fines de auditoría.</param>
        /// <exception cref="UserFriendlyException">Si la solicitud no existe o el usuario no tiene permisos.</exception>
        public async Task UpdateStatusAsync(int requestId, string newStatus, string reason)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            // Validación de propiedad: Un operador no puede editar lo que no tiene asignado
            if (_currentUserProvider.Role == UserRole.Operator.ToString() &&
                request.AssignedUserId != _currentUserProvider.UserId)
            {
                throw new UserFriendlyException("No tienes permiso para modificar esta solicitud.");
            }

            var oldStatus = request.Status;
            var nextStatus = Enum.Parse<RequestStatus>(newStatus);

            request.Status = nextStatus;

            // Generación de registro de auditoría (Requerimiento 2.4)
            var history = new RequestHistory
            {
                RequestId = requestId,
                PreviousStatus = oldStatus,
                NewStatus = nextStatus,
                ChangeReason = reason
            };

            _context.RequestHistory.Add(history);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud {Code} cambiada de {Old} a {New}", request.Code, oldStatus, nextStatus);
        }

        /// <summary>
        /// Asigna una solicitud a un usuario operador específico. Solo permitido para Administradores.
        /// </summary>
        public async Task AssignRequestAsync(int requestId, int userId)
        {
            if (_currentUserProvider.Role != UserRole.Administrator.ToString())
                throw new UserFriendlyException("Solo los administradores pueden asignar solicitudes.");

            var request = await _context.Requests.FindAsync(requestId)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            request.AssignedUserId = userId;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Ejecuta un borrado lógico (Soft Delete) de la solicitud mediante el flag IsDeleted.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var request = await _context.Requests.FindAsync(id)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            request.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Recupera el detalle de una solicitud individual incluyendo información del usuario asignado.
        /// </summary>
        public async Task<RequestDto?> GetByIdAsync(int id)
        {
            return await _context.Requests
                .Include(r => r.AssignedUser)
                .Where(r => r.Id == id)
                .Select(r => new RequestDto(
                    r.Id, r.Code, r.Title, r.Description,
                    r.Status.ToString(), r.Priority.ToString(), r.Area.ToString(),
                    r.AssignedUser != null ? r.AssignedUser.Username : null,
                    r.CreatedDate ?? DateTime.Now))
                .FirstOrDefaultAsync();
        }
    }
}