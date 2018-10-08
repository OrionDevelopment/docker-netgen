using System.Collections.Generic;

namespace docker_netgen.Template.Core
{
    public interface IImportParser
    {
        List<string> GetImportsFromCode(string code);
    }
}