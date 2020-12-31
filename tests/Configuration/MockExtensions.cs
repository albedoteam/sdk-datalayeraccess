using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    public static class MockExtensions
    {
        public static IServiceCollection AddMock<T>(
            this IServiceCollection services,
            IMockCreator<T> mockCreator,
            params Guid[] ids)
            where T : class
        {
            var mock = mockCreator.Create(ids);
            services.SwapTransient(provider => mock.Object);

            return services;
        }

        public static T Get<T>(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<T>();
        }
        
        /// <summary>
        /// Removes all registered <see cref="ServiceLifetime.Transient"/> registrations of <see cref="TService"/> and adds a new registration which uses the <see cref="Func{IServiceProvider, TService}"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service interface which needs to be placed.</typeparam>
        /// <param name="services"></param>
        /// <param name="implementationFactory">The implementation factory for the specified type.</param>
        private static void SwapTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory)
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();
                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }

            services.AddTransient(typeof(TService), (sp) => implementationFactory(sp));
        }
    }
}