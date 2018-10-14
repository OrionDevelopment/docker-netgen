using System;
using docker_netgen.Template.Core;

namespace docker_netgen.Template
{
    public class SimpleWriter : IWriter
    {
        public string Written { get; private set; } = "";
        
        public void Emit(string toEmit)
        {
            Written += toEmit;
        }

        public void EmitNewline(string toEmit)
        {
            Emit(toEmit);
            Emit(Environment.NewLine);
        }
    }
}