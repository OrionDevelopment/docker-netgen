using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using docker_netgen.Template.Core;
using Docker.DotNet.Models;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

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
        private readonly IImportParser _importParser;
        private Script _script;

        public Template(string scriptContents, IImportParser importParser)
        {
            _importParser = importParser;
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
                .WithSourceResolver(ScriptSourceResolver.Default.WithBaseDirectory(Directory.GetCurrentDirectory()));

            this._script = CSharpScript.Create(scriptContents, options, typeof(IGlobal));
        }

        /// <inheritdoc />
        public async Task Execute(IList<ContainerInspectResponse> containers, IWriter writer)
        {
            var global = new Global(containers, writer);
            await this._script.RunAsync(global, exception => throw exception, CancellationToken.None);
        }
    }
}