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

        public async Task<int> CreateAsync(CreateRequestDto dto)
        {
            _logger.LogInformation("Creando nueva solicitud: {Title}", dto.Title);

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

        public async Task<IEnumerable<RequestDto>> GetAllAsync()
        {
            var query = _context.Requests
                .Include(r => r.AssignedUser)
                .AsQueryable();

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

        public async Task UpdateStatusAsync(int requestId, string newStatus, string reason)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Id == requestId)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            if (_currentUserProvider.Role == UserRole.Operator.ToString() &&
                request.AssignedUserId != _currentUserProvider.UserId)
            {
                throw new UserFriendlyException("No tienes permiso para modificar esta solicitud.");
            }

            var oldStatus = request.Status;
            var nextStatus = Enum.Parse<RequestStatus>(newStatus);

            request.Status = nextStatus;

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

        public async Task AssignRequestAsync(int requestId, int userId)
        {
            if (_currentUserProvider.Role != UserRole.Administrator.ToString())
                throw new UserFriendlyException("Solo los administradores pueden asignar solicitudes.");

            var request = await _context.Requests.FindAsync(requestId)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            request.AssignedUserId = userId;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _context.Requests.FindAsync(id)
                ?? throw new UserFriendlyException("Solicitud no encontrada.");

            request.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

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