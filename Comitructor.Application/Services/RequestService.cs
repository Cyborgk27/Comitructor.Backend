using Comitructor.Application.Dtos;
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
                DueDate = dto.DueDate ?? DateTime.Now.AddDays(2),
                AssignedUserId = dto.AssignedUserId
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return request.Id;
        }

        /// <summary>
        /// Actualiza los datos generales de una solicitud existente respetando las reglas del dominio.
        /// </summary>
        /// <param name="dto">Datos de la solicitud a actualizar.</param>
        /// <returns>ID de la solicitud actualizada.</returns>
        public async Task<int> UpdateAsync(UpdateRequestDto dto)
        {
            _logger.LogInformation("Iniciando actualización de solicitud {Id}", dto.Id);

            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Id == dto.Id)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            // 1. Validación de reglas de negocio desde la entidad
            if (!request.IsEditable())
            {
                throw new UserFriendlyException($"La solicitud {request.Code} no se puede editar porque su estado es {request.Status}.");
            }

            // 2. Validación de seguridad (Permisos por Rol)
            if (_currentUserProvider.Role == UserRole.Operator.ToString() &&
                request.AssignedUserId != _currentUserProvider.UserId &&
                request.CreatedBy != _currentUserProvider.UserId)
            {
                throw new UserFriendlyException("No tienes permisos suficientes para modificar esta solicitud.");
            }

            // 3. Actualización de campos
            request.Title = dto.Title;
            request.Description = dto.Description;
            request.Priority = Enum.Parse<RequestPriority>(dto.Priority);
            request.Area = Enum.Parse<RequestArea>(dto.Area);
            request.DueDate = dto.DueDate;
            request.AssignedUserId = dto.AssignedUserId;

            // 4. Auditoría de BaseEntity
            request.LastModifiedBy = _currentUserProvider.UserId;
            request.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Solicitud {Code} actualizada por el usuario {UserId}", request.Code, _currentUserProvider.UserId);

            return request.Id;
        }

        /// <summary>
        /// Obtiene solicitudes filtradas, paginadas y con lógica de seguridad aplicada.
        /// </summary>
        public async Task<PagedResponseDto<RequestDto>> GetAllAsync(RequestFilterDto filter)
        {
            _logger.LogInformation("Consultando solicitudes con filtros: {@Filter}", filter);

            var query = _context.Requests
                .Include(r => r.AssignedUser)
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            // 1. Lógica de Seguridad (Roles)
            if (_currentUserProvider.Role == UserRole.Operator.ToString())
            {
                query = query.Where(r => r.AssignedUserId == _currentUserProvider.UserId);
            }

            // 2. Filtros Dinámicos
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(r => r.Title.ToLower().Contains(term) ||
                                         r.Code.ToLower().Contains(term) ||
                                         r.Description.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                if (Enum.TryParse<RequestStatus>(filter.Status, true, out var statusEnum))
                    query = query.Where(r => r.Status == statusEnum);
            }

            if (!string.IsNullOrWhiteSpace(filter.Priority))
            {
                if (Enum.TryParse<RequestPriority>(filter.Priority, true, out var priorityEnum))
                    query = query.Where(r => r.Priority == priorityEnum);
            }

            // 3. Conteo Total (Antes de paginar)
            var totalCount = await query.CountAsync();

            // 4. Paginación y Proyección
            var items = await query
                .OrderByDescending(r => r.Id)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new RequestDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Title = r.Title,
                    Description = r.Description,
                    Status = r.Status.ToString(),
                    Priority = r.Priority.ToString(),
                    Area = r.Area.ToString(),
                    AssignedUserName = r.AssignedUser != null ? r.AssignedUser.Username : "Sin asignar",
                    CreatedDate = r.CreatedDate ?? DateTime.Now,
                    DueDate = r.DueDate
                })
                .ToListAsync();

            return new PagedResponseDto<RequestDto>
            {
                Items = items,
                TotalCount = totalCount
            };
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
                .Select(r => new RequestDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Title = r.Title,
                    Description = r.Description,
                    Status = r.Status.ToString(),
                    Priority = r.Priority.ToString(),
                    Area = r.Area.ToString(),
                    AssignedUserId = r.AssignedUserId,
                    AssignedUserName = r.AssignedUser != null ? r.AssignedUser.Username : null,
                    DueDate = r.DueDate,
                    CreatedDate = r.CreatedDate ?? DateTime.Now
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Recupera una lista optimizada de usuarios activos para su visualización en componentes de selección.
        /// </summary>
        /// <returns>Una colección de <see cref="UserResponseDto"/> con datos mínimos de identidad.</returns>
        /// <remarks>
        /// Utiliza <see cref="QueryableExtensions.AsNoTracking"/> para mejorar el rendimiento al omitir el seguimiento 
        /// en el Change Tracker de EF Core. Filtra automáticamente usuarios con borrado lógico (<c>IsDeleted</c>).
        /// </remarks>
        public async Task<IEnumerable<UserResponseDto>> GetUsersForSelectAsync()
        {
            _logger.LogInformation("Iniciando consulta de usuarios para componentes de selección.");

            try
            {
                var users = await _context.Users
                    .AsNoTracking()
                    .Where(u => !u.IsDeleted)
                    .Select(u => new UserResponseDto
                    {
                        Id = u.Id,
                        UserName = u.Username
                    })
                    .ToListAsync();

                _logger.LogInformation("Se recuperaron {Count} usuarios activos con éxito.", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar recuperar la lista de usuarios para selección.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene el historial de una solicitud incluyendo los datos del usuario asignado a la petición original.
        /// </summary>
        /// <param name="requestId">Identificador único de la solicitud.</param>
        /// <returns>Lista de historial con el nombre del usuario responsable.</returns>
        public async Task<IEnumerable<RequestHistoryDto>> GetHistoryAsync(int requestId)
        {
            return await _context.RequestHistory
                .Where(h => h.RequestId == requestId)
                .OrderByDescending(h => h.CreatedDate)
                .Include(h => h.Request)
                    .ThenInclude(r => r.AssignedUser)
                .Select(h => new RequestHistoryDto
                {
                    Id = h.Id,
                    PreviousStatus = h.PreviousStatus.ToString(),
                    NewStatus = h.NewStatus.ToString(),
                    ChangeReason = h.ChangeReason,
                    CreatedDate = h.CreatedDate ?? DateTime.UtcNow,
                    UserName = h.Request.AssignedUser != null
                               ? h.Request.AssignedUser.Username
                               : "Sistema"
                })
                .ToListAsync();
        }

        /// <summary>
        /// Calcula las métricas de las solicitudes basadas en su estado, prioridad y fecha de vencimiento.
        /// </summary>
        public async Task<RequestSummaryDto> GetSummaryAsync()
        {
            var now = DateTime.UtcNow;

            // Obtenemos todos los datos necesarios en una sola pasada para eficiencia
            var stats = await _context.Requests
                .Select(r => new { r.Status, r.Priority, r.DueDate })
                .ToListAsync();

            return new RequestSummaryDto
            {
                TotalRequests = stats.Count,

                OpenRequests = stats.Count(r => r.Status == RequestStatus.New ||
                                               r.Status == RequestStatus.InProgress),

                CriticalRequests = stats.Count(r => r.Priority == RequestPriority.Critical),

                // Vencidas: Fecha pasada y que no estén cerradas/canceladas
                OverdueRequests = stats.Count(r => r.DueDate < now &&
                                                  r.Status != RequestStatus.Closed &&
                                                  r.Status != RequestStatus.Cancelled),

                ClosedRequests = stats.Count(r => r.Status == RequestStatus.Closed ||
                                                r.Status == RequestStatus.Cancelled)
            };
        }
    }
}