using CentralDeErros.Services;
using CentralDeErros.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CentralDeErros.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services)
        {

            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<IEnvironmentService, EnvironmentService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<MicrosserviceService>();
            services.AddScoped<TokenGeneratorService>();

            return services;

        }
    }
}
