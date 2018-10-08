using System.Threading.Tasks;
using docker_netgen.Configuration;
using Docker.DotNet.Models;

namespace docker_netgen.Runtime.Core
{
    public interface IDockerGenGenerator
    {
        Task Generate(DockerGenConfiguration configuration, ContainerListResponse container);
    }
}