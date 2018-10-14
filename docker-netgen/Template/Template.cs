using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using docker_netgen.Template.Core;
using docker_netgen.Utils;
using Docker.DotNet.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;

namespace docker_netgen.Template
{
    /// <summary>
    /// Represents an instance of a Template in the normal DockerGen.
    /// Will load CSharp scripts using the normal runtime (requires the SDK to be installed).
    ///
    /// Before running the script, imports and references are loaded by asking the relevant parsers.
    /// As global variables for the script an instance of IGlobal is used.
    /// </summary>
    public class Template : ITemplate
    {
        private readonly IConfiguration _configuration;
        private readonly IImportParser _importParser;
        private readonly IEnumerable<Assembly> _referencedAssemblies;
        private Microsoft.CodeAnalysis.Scripting.Script _script;

        public Template(string scriptContents, IImportParser importParser, IConfiguration configuration, IEnumerable<Assembly> referencedAssemblies)
        {
            _importParser = importParser;
            _configuration = configuration;
            _referencedAssemblies = referencedAssemblies;
            this.Initialize(scriptContents);
        }

        /// <summary>
        /// Sets up the new script.
        /// </summary>
        /// <param name="scriptContents">The contents of the script.</param>
        private void Initialize(string scriptContents)
        {
            var imports = _importParser.GetImportsFromCode(scriptContents);

            var options = ScriptOptions.Default
                .AddImports(imports)
                .AddReferences(this._referencedAssemblies);

            this._script = CSharpScript.Create(scriptContents, options, typeof(IGlobal));
            var errors = this._script.Compile();
            if (errors != null && errors.Any())
                throw new Exception(@"Failed to compile script.

Following errors detected: 
" + errors.Select(d => d.ToString()).Aggregate((s1, s2) => s1.Any() ? "" : "   * " + s1 + Environment.NewLine + "   * " + s2));
        }

        /// <inheritdoc />
        public async Task Execute(IList<ContainerInspectResponse> containers, IWriter writer)
        {
            var currentContainer = containers.First(c => c.ID.Equals(DockerUtils.GetCurrentContainerId()));
            var groupedContainersByDomain = containers
                    .Where(c => c.Config.Env.Any(e => e.StartsWith("VIRTUAL_HOST")))
                    .GroupBy(c => c.Config.Env.FirstOrDefault(s => s.StartsWith("VIRTUAL_HOST="))?.Replace("VIRTUAL_HOST=", ""))
                    .ToList();

            foreach (IGrouping<string,ContainerInspectResponse> grouping in groupedContainersByDomain)
            {
                var domain = grouping.Key;
                domain = domain.StartsWith("~") ? domain.Sha1() : domain;

                foreach (ContainerInspectResponse response in grouping)
                {
                    foreach (string currentContainerNetworkName in currentContainer.NetworkSettings.Networks.Keys)
                    {
                        foreach (string containerNetworkName in response.NetworkSettings.Networks.Keys)
                        {
                            if (containerNetworkName != "ingress" &&
                                (currentContainerNetworkName == containerNetworkName) || containerNetworkName == "host")
                            {
                                if (response.Config.ExposedPorts.Count == 1)
                                {
                                    
                                }
                                else
                                {
                                    
                                }
                            }
                            else
                            {
                                
                            }
                        }
                    }
                }
            }
            
            var global = new Global(containers, (SimpleWriter) writer, _configuration);
            await this._script.RunAsync(global, exception => throw exception, CancellationToken.None);
        }
    }
}