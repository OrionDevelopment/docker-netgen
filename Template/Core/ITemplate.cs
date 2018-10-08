using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace docker_netgen.Template.Core
{
    /// <summary>
    /// Represents a single Template in the engine.
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Executes the template.
        /// </summary>
        /// <param name="containers">The list of containers currently attached.</param>
        /// <param name="writer">The writer.</param>
        Task Execute(IList<ContainerInspectResponse> containers, IWriter writer);
    }
}