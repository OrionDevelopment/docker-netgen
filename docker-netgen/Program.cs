using System;
using System.Threading.Tasks;
using docker_netgen.Runtime.Core;
using docker_netgen.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace docker_netgen
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var services = ServiceUtils.BuildServices(args);
            await services.GetService<IDockerGenRuntime>()
                .Run();
        }
    }
}