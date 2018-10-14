using docker_netgen.Template.Core;

namespace docker_netgen.Utils
{
    public static class IWriterExtensions
    {
        public static void EmitNewLine(this IWriter writer, string toString)
        {
            writer.EmitNewline(toString);
        }
    }
}