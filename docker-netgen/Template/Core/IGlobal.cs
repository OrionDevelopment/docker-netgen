using System.Collections.Generic;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;

namespace docker_netgen.Template.Core
{
    /// <summary>
    /// Represents the global variables that can be accessed in the template.
    /// </summary>
    public interface IGlobal
    {
        IList<ContainerInspectResponse> Containers { get; }
        
        IWriter Writer { get; }
        
        IConfiguration Configuration { get; }
    }
}