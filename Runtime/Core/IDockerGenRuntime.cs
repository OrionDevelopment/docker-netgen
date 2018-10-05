using System.Threading.Tasks;

namespace docker_netgen.Runtime.Core
{
    public interface IDockerGenRuntime
    {
        Task Run();
    }
}