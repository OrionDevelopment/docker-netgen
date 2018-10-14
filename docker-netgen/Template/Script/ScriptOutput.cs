using docker_netgen.Template.Core;

namespace docker_netgen.Template.Script
{
    public class ScriptOutput
    {
        public static void Emit(IWriter writer, object toEmit)
        {
            writer.Emit(toEmit.ToString());
        }
        
        public static void EmitNewLine(IWriter writer, object toEmit)
        {
            writer.EmitNewline(toEmit.ToString());
        }
    }
}