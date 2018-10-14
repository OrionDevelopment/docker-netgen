using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using docker_netgen.Configuration;
using docker_netgen.Runtime.Core;
using docker_netgen.Template.Core;
using docker_netgen.Utils;
using Docker.DotNet;

namespace docker_netgen.Runtime.Component
{
    public class TimerBasedTriggeringComponent : DockerGenComponentBase
    {
        public TimerBasedTriggeringComponent(IDockerClient client, ITemplateFactory templateFactory, IEnumerable<DockerGenConfiguration> configurations) : base(client, templateFactory)
        {
            this.Configurations = configurations.Where(c => c.Interval > TimeSpan.Zero && c.Watch == false);
        }

        public override async Task Run()
        {
            this.Configurations.ForEach(config =>
            {
                var configTask = new Task(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(config.Wait);
                        OnTriggered(config);
                        Thread.Sleep(config.Interval);
                    }
                    
                    // ReSharper disable once FunctionNeverReturns
                    // Intended since this should just always run.
                });
                
                Threads.Add(config, configTask);
            });
            
            Threads.Values.ForEach(t => t.Start());

            Task.WaitAll(Threads.Values.ToArray());
        }
    }
}
