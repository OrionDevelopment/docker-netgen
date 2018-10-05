using System;
using docker_netgen.Runtime.Core;
using Microsoft.Extensions.DependencyInjection;

namespace docker_netgen
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBuilder = new ServiceCollection();
            
            Startup.ConfigureServices(serviceBuilder);

            var services = serviceBuilder.BuildServiceProvider();
            services.GetService<IDockerGenRuntime>().Run();
        }
    }
}