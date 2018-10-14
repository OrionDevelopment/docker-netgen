using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using docker_netgen.Configuration;
using docker_netgen.Template.Core;
using docker_netgen.Utils;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace docker_netgen.Runtime.Component
{
    public class EventWatchingComponent : DockerGenComponentBase
    {
        public EventWatchingComponent(IDockerClient client, ITemplateFactory templateFactory, IEnumerable<DockerGenConfiguration> configurations) : base(client, templateFactory)
        {
            this.Configurations = configurations.Where(c => c.Watch);
        }

        public override async Task Run()
        {
            this.Configurations.ForEach(config =>
            {
                var configTask = new Task(async () =>
                {
                    await Client.System.MonitorEventsAsync(new ContainerEventsParameters(), new Progress<JSONMessage>(
                        (m) =>
                        {
                            Thread.Sleep(config.Wait);
                            OnTriggered(config);
                        }));
                });
                
                Threads.Add(config, configTask);
            });
            
            Threads.Values.ForEach(t => t.Start());

            Task.WaitAll(Threads.Values.ToArray());
        }
    }
}