using System;
using Microsoft.Extensions.DependencyInjection;

namespace docker_netgen.Utils
{
    public class ServiceUtils
    {
        public static IServiceProvider BuildServices(params string[] args)
        {
            var serviceBuilder = new ServiceCollection();
            
            Startup.ConfigureServices(serviceBuilder, args);

            var services = serviceBuilder.BuildServiceProvider();
            return services;
        }
    }
}