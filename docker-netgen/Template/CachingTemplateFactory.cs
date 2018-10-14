using System;
using System.Collections.Generic;
using docker_netgen.Template.Core;
using docker_netgen.Utils;

namespace docker_netgen.Template
{
    public class CachingTemplateFactory : ITemplateFactory
    {
        private readonly ITemplateFactory _other;
        
        private readonly Dictionary<string, ITemplate> _rawCodeCache = new Dictionary<string, ITemplate>();
        private readonly Dictionary<Uri, ITemplate> _uriCache = new Dictionary<Uri, ITemplate>();
        private readonly Dictionary<string, ITemplate> _pathCache = new Dictionary<string, ITemplate>();

        public CachingTemplateFactory(ITemplateFactory other)
        {
            _other = other;
        }

        public ITemplate GenerateFromRawCode(string code)
        {
            return _rawCodeCache.GetOrAdd(code, () => _other.GenerateFromRawCode(code));
        }

        public ITemplate GenerateFromUri(Uri codeLocation)
        {
            return _uriCache.GetOrAdd(codeLocation, () => _other.GenerateFromUri(codeLocation));
        }

        public ITemplate GenerateFromPath(string path)
        {
            return _pathCache.GetOrAdd(path, () => _other.GenerateFromPath(path));
        }
    }
}