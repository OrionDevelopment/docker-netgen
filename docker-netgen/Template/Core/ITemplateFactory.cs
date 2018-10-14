using System;

namespace docker_netgen.Template.Core
{
    public interface ITemplateFactory
    {
        /// <summary>
        /// Generates a new template from the given code.
        /// </summary>
        /// <param name="code">The code to generate a new template from</param>
        /// <returns>The template instance.</returns>
        ITemplate GenerateFromRawCode(string code);

        /// <summary>
        /// Generates a new template by locating the code stored in the URI and generating the template from that.
        /// </summary>
        /// <param name="codeLocation">The URI pointing to the code.</param>
        /// <returns>The template instance.</returns>
        ITemplate GenerateFromUri(Uri codeLocation);

        /// <summary>
        /// Generates a new template by locating the code stored in the path and generating the template from that.
        /// </summary>
        /// <param name="path">The path to load the code from.</param>
        /// <returns>The template instance.</returns>
        ITemplate GenerateFromPath(string path);
    }
}