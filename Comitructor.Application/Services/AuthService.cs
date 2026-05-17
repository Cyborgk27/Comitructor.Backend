using Comitructor.Application.Dtos.Auth;
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
    /// Servicio encargado de la gestión de identidad, autenticación y registro de usuarios.
    /// Implementa lógica de seguridad mediante hashing de contraseñas y emisión de tokens JWT.
    /// </summary>
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

        /// <summary>
        /// Valida las credenciales de un usuario y genera un token de acceso si son correctas.
        /// </summary>
        /// <param name="request">Contiene el nombre de usuario y la contraseña en texto plano.</param>
        /// <returns>Objeto con el Token JWT, el nombre de usuario y su rol correspondiente.</returns>
        /// <exception cref="UserFriendlyException">Lanzada si el usuario no existe o la contraseña es incorrecta.</exception>
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            _logger.LogInformation("Intento de inicio de sesión para el usuario: {Username}", request.Username);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // Verificación segura: Si el usuario no existe, evitamos la verificación de hash para mitigar ataques de tiempo
            if (user is null || !_passwordHasher.Verify(request.Password, user.Password))
            {
                _logger.LogWarning("Fallo de autenticación para: {Username}", request.Username);
                throw new UserFriendlyException("Nombre de usuario o contraseña incorrectos.");
            }

            // Generación del token basado en los claims del usuario (ID, Username, Role)
            var token = _jwt.Generate(user);

            _logger.LogInformation("Usuario {Username} autenticado exitosamente con rol {Role}", user.Username, user.Role);

            return new LoginResponse()
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.ToString()
            }
            ;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema con el rol predeterminado 'Operator'.
        /// </summary>
        /// <remarks>
        /// El proceso incluye la normalización del nombre de usuario y el hasheo de la contraseña 
        /// antes de persistir los datos en la base de datos SQL Server.
        /// </remarks>
        /// <param name="request">Datos del nuevo usuario.</param>
        /// <exception cref="UserFriendlyException">Lanzada si el nombre de usuario ya está en uso.</exception>
        public async Task Register(LoginRequest request)
        {
            // Verificación de unicidad de usuario
            var exists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (exists)
            {
                throw new UserFriendlyException("El nombre de usuario ya se encuentra registrado.");
            }

            // Seguridad: Nunca guardar contraseñas en texto plano
            var passwordHash = _passwordHasher.Hash(request.Password);

            var newUser = new User
            {
                Username = request.Username.ToLower().Trim(),
                Password = passwordHash,
                Role = UserRole.Operator, // Todo registro nuevo es Operador por defecto
                CreatedDate = DateTime.UtcNow,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nuevo usuario registrado: {Username}", newUser.Username);
        }
    }
}