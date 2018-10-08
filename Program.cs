using System;
using System.Threading.Tasks;
using docker_netgen.Runtime.Core;
using Microsoft.Extensions.DependencyInjection;

namespace docker_netgen
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceBuilder = new ServiceCollection();
            
            Startup.ConfigureServices(serviceBuilder, args);

            var services = serviceBuilder.BuildServiceProvider();
            await services.GetService<IDockerGenRuntime>()
                .Run();
        }
    }
}