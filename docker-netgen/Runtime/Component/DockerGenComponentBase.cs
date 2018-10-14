using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using docker_netgen.Configuration;
using docker_netgen.Runtime.Core;
using docker_netgen.Template;
using docker_netgen.Template.Core;
using docker_netgen.Utils;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace docker_netgen.Runtime.Component
{
    public abstract class DockerGenComponentBase : IDockerGenComponent
    {
        protected DockerGenComponentBase(IDockerClient client, ITemplateFactory templateFactory)
        {
            Client = client;
            _templateFactory = templateFactory;
        }

        protected IDockerClient Client { get; }
        protected Dictionary<DockerGenConfiguration, Task> Threads { get; } = new Dictionary<DockerGenConfiguration, Task>();
        protected IEnumerable<DockerGenConfiguration> Configurations { get; set; }
        
        private readonly ITemplateFactory _templateFactory;
        
        protected async void OnTriggered(DockerGenConfiguration configuration)
        {
            var containers = await GetContainers(configuration);
            var writer = new SimpleWriter();
            var template = _templateFactory.GenerateFromPath(configuration.Template);

            var containerInspections = containers
                .Select(c => c.ID)
                .Select(containerId => Client.Containers.InspectContainerAsync(containerId).Result)
                .ToList();
            
            await template.Execute(containerInspections, writer);

            var generatedContents = writer.Written;
            
            var targetFile = configuration.Destination;
            if (!Path.IsPathRooted(targetFile))
            {
                targetFile = Path.Combine(Directory.GetCurrentDirectory(), targetFile);
            }

            var currentFileContents = "";
            if (File.Exists(targetFile))
            {
                currentFileContents = File.ReadAllText(targetFile);
            }
            
            if (currentFileContents == "" || currentFileContents != generatedContents)
            {
                //Change detected lets write the contents
                File.Delete(targetFile);
                File.Create(targetFile);
                await File.WriteAllTextAsync(targetFile, generatedContents);
                
                await OnChanged(configuration);
            }
        }

        private async Task OnChanged(DockerGenConfiguration configuration)
        {
            await configuration.NotifyContainers.Keys.ForEachAsync(async containerId =>
            {
                await Client.Containers.KillContainerAsync(containerId, new ContainerKillParameters()
                {
                    Signal = configuration.NotifyContainers[containerId].ToString()
                });
            });
        }

        private async Task<IList<ContainerListResponse>> GetContainers(DockerGenConfiguration configuration)
        {
            var all = await Client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = configuration.IncludeStopped
            });

            if (configuration.OnlyPublished)
            {
                all = all.Where(c => c.Ports.Any()).ToList();
            }

            return all;
        }

        public abstract Task Run();
    }
}