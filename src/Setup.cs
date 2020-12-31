using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public static class Setup
    {
        public static IServiceCollection AddDataLayerAccess(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DbSettings>(configuration.GetSection(nameof(DbSettings)));
            services.AddSingleton<IDbSettings>(sp =>
                sp.GetRequiredService<IOptions<DbSettings>>().Value);

            services.AddTransient<IRepository, MongoRepository>();

            return services;
        }
    }
}