using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    public abstract class BaseServiceFixture : IDisposable
    {
        private readonly ServiceProvider _bsp;
        private readonly IServiceScope _scope;

        protected BaseServiceFixture()
        {
            var builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection(new Dictionary<string, string>());
            var config = builder.Build();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(sp => config);

            _bsp = services.BuildServiceProvider();
            _scope = _bsp.CreateScope();
        }
        
        public abstract void Configure(IServiceCollection services);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            _scope.Dispose();
            _bsp.Dispose();
        }
    }
}