using System;
using System.Collections.Generic;
using docker_netgen.Template.Core;
using Docker.DotNet.Models;
using Microsoft.Extensions.Configuration;

namespace docker_netgen.Template
{
    public class Global : IGlobal
    {
        public Global(IList<ContainerInspectResponse> containers, IWriter writer, IConfiguration configuration)
        {
            Containers = containers;
            Writer = writer;
            Configuration = configuration;
        }

        public IList<ContainerInspectResponse> Containers { get; }
        public IWriter Writer { get; }
        public IConfiguration Configuration { get; }
    }
}