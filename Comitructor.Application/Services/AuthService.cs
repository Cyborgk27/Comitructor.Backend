using Comitructor.Application.Dtos;
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
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtProvider _jwt;
        private readonly ILogger<AuthService> _logger;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            ILogger<AuthService> logger,
            ApplicationDbContext context,
            IJwtProvider jwt,
            IPasswordHasher passwordHasher)
        {
            _logger = logger;
            _context = context;
            _jwt = jwt;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            _logger.LogInformation("Intento de inicio de sesión para el usuario: {Username}", request.Username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            var isValid = !_passwordHasher.Verify(request.Password, user!.Password);

            if (user is null || isValid)
            {
                _logger.LogWarning("Fallo de autenticación para: {Username}", request.Username);
                throw new UserFriendlyException("Nombre de usuario o contraseña incorrectos.");
            }

            var token = _jwt.Generate(user);

            _logger.LogInformation("Usuario {Username} autenticado exitosamente con rol {Role}", user.Username, user.Role);

            return new LoginResponse(
                Token: token,
                Username: user.Username,
                Role: user.Role.ToString()
            );
        }

        public async Task Register(LoginRequest request)
        {
            var exists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (exists)
            {
                throw new UserFriendlyException("El nombre de usuario ya se encuentra registrado.");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            var newUser = new User
            {
                Username = request.Username.ToLower().Trim(),
                Password = passwordHash,
                Role = UserRole.Operator,
                CreatedDate = DateTime.UtcNow,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nuevo usuario registrado: {Username}", newUser.Username);
        }
    }
}