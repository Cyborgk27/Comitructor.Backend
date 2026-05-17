using Comitructor.Domain.Interfaces;
using Comitructor.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using BC = BCrypt.Net.BCrypt;

namespace Comitructor.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly int _workFactor;
        public PasswordHasher(IOptions<InfrastructureSettings> settings)
        {
            _workFactor = settings.Value.Security.HashWorkFactor;
        }
        public string Hash(string password)
        {
            return BC.HashPassword(password, _workFactor);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BC.Verify(password, passwordHash);
        }
    }
}
