namespace docker_netgen.Template.Core
{
    /// <summary>
    /// Writes the results of the script parsing to the desired file.
    /// </summary>
    public interface IWriter
    {
        /// <summary>
        /// Emits the given string without appending a newline.
        /// </summary>
        /// <param name="toEmit">The string to append to the output.</param>
        void Emit(string toEmit);

        /// <summary>
        /// Emits the given string and a newline character.
        /// </summary>
        /// <param name="toEmit">The string to append to the output, before appending a newline character.</param>
        void EmitNewline(string toEmit);
    }
}