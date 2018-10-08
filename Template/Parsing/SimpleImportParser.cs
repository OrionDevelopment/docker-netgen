using System;
using System.Collections.Generic;
using System.Linq;
using docker_netgen.Template.Core;

namespace docker_netgen.Template.Parsing
{
    public class SimpleImportParser : IImportParser
    {
        
        public List<string> GetImportsFromCode(string code) => code.Split(Environment.NewLine)
            .TakeWhile(s => s.StartsWith("//# using"))
            .Select(s => s.Substring(9).Trim())
            .Where(s => s.Any() && s.EndsWith(";"))
            .Select(s => s.Replace(";", ""))
            .ToList();
    }
}

//#