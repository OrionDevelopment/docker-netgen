using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using docker_netgen.Template.Core;
using Microsoft.Extensions.Configuration;

namespace docker_netgen.Template
{
    public class SimpleTemplateFactory : ITemplateFactory
    {

        private readonly IConfiguration _configuration;
        private readonly IImportParser _importParser;
        private readonly IEnumerable<Assembly> _referencedAssemblies;

        public SimpleTemplateFactory(IImportParser importParser, IConfiguration configuration, IEnumerable<Assembly> referencedAssemblies)
        {
            _importParser = importParser;
            _configuration = configuration;
            _referencedAssemblies = referencedAssemblies;
        }

        public ITemplate GenerateFromRawCode(string code)
        {
            return new Template(code, _importParser, _configuration, _referencedAssemblies);
        }

        public ITemplate GenerateFromUri(Uri codeLocation)
        {
            var textFromFile = (new WebClient()).DownloadString(codeLocation);
            return GenerateFromRawCode(textFromFile);
        }

        public ITemplate GenerateFromPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }

            return GenerateFromRawCode(File.ReadAllText(path));
        }
    }
}