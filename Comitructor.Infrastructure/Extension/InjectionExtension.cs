using Comitructor.Infrastructure.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Comitructor.Infrastructure.Extension
{
    public static class InjectionExtension
    {
        public static IServiceCollection AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var infraSettings = new InfrastructureSettings();

            configuration.GetSection("Infrastructure").Bind(infraSettings);

            services.AddCors(options =>
            {
                options.AddPolicy("ComiTructorCorsPolicy", policy =>
                {
                    var cors = infraSettings.Cors;
                    if (cors != null)
                    {
                        policy.WithOrigins(cors.AllowedOrigins)
                              .WithMethods(cors.AllowedMethods)
                              .WithHeaders(cors.AllowedHeaders)
                              .AllowCredentials();
                    }
                });
            });

            var jwtSettings = infraSettings.Security.Jwt;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddHttpContextAccessor();

            services.Configure<InfrastructureSettings>(configuration.GetSection("Infrastructure"));

            return services;
        }
    }
}
