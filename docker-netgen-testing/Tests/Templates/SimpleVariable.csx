using static docker_netgen.Template.Script.ScriptOutput;
using docker_netgen.Template.Core;
using Microsoft.Extensions.Configuration;

bool value = Configuration.GetSection("TestInput").Get<bool>();
Emit(Writer, !value);