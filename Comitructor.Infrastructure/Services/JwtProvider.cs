using Comitructor.Domain.Entities;
using Comitructor.Domain.Interfaces;
using Comitructor.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Comitructor.Infrastructure.Services
{
    public class JwtProvider : IJwtProvider
    {
        private readonly InfrastructureSettings _settings;

        public JwtProvider(IOptions<InfrastructureSettings> settings)
        {
            _settings = settings.Value;
        }
        public string Generate(User user)
        {
            var jwt = _settings.Security.Jwt;
            var secretKey = _settings.Security.Jwt.Key;
            var issuer = _settings.Security.Jwt.Issuer;
            var audience = _settings.Security.Jwt.Audience;
            var duration = _settings.Security.Jwt.DurationInMinutes;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duration),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
