using System;
using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public static class Setup
    {
        public static IServiceCollection AddDataLayerAccess(
            this IServiceCollection services,
            Action<IDbSettings> configureDb)
        {
            IDbSettings dbSettings = new DbSettings();
            configureDb.Invoke(dbSettings);

            services.AddSingleton(dbSettings);
            services.AddScoped(typeof(IDbContext<>), typeof(DbContext<>));

            return services;
        }
    }
}