using System;
using System.Collections.Generic;
using docker_netgen.Template;
using docker_netgen.Template.Core;
using docker_netgen.Template.Parsing;
using docker_netgen.Utils;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace docker_netgen_testing
{
    public class TemplateTest
    {
        [Fact]
        public void SimpleOutput()
        {
            RunTest("SimpleOutput", "This is a Test");
        }

        [Fact]
        public void LoadFromConfiguration()
        {
            RunTest("SimpleConfigurationLoad", "True");
        }

        [Fact]
        public void LoadWithVariable()
        {
            RunTest("SimpleVariable", "False");
        }

        [Fact]
        public void NGinx()
        {
            RunTest("nginx", "");
        }

        private static void RunTest(string fileName, string expected)
        {
            var services = ServiceUtils.BuildServices();
            var templateFactory = services.GetRequiredService<ITemplateFactory>();
            var template = templateFactory.GenerateFromPath($@"Tests/Templates/{fileName}.csx");

            var writer = new SimpleWriter();
            template.Execute(new List<ContainerInspectResponse>(), writer);
            var contents = writer.Written;
            Assert.Equal(expected, contents);
        }
    }
}