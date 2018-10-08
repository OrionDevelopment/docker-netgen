using System;
using System.IO;
using System.Net;
using docker_netgen.Template.Core;

namespace docker_netgen.Template
{
    public class SimpleTemplateFactory : ITemplateFactory
    {

        private readonly IImportParser _importParser;

        public SimpleTemplateFactory(IImportParser importParser)
        {
            _importParser = importParser;
        }

        public ITemplate GenerateFromRawCode(string code)
        {
            return new Template(code, _importParser);
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