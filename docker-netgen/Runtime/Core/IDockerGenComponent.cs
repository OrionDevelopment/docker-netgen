using System.Threading.Tasks;

namespace docker_netgen.Runtime.Core
{
    public interface IDockerGenComponent
    {
        Task Run();
    }
}