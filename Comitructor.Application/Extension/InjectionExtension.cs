using Comitructor.Application.Interfaces;
using Comitructor.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Comitructor.Application.Extension
{
    public static class InjectionExtension
    {
        public static IServiceCollection AddInjectionIApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
