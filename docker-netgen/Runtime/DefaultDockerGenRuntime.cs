using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using docker_netgen.Runtime.Core;
using docker_netgen.Utils;

namespace docker_netgen.Runtime
{
    //Represents the default docker runtime.
    public class DefaultDockerGenRuntime : IDockerGenRuntime
    {
        private IEnumerable<IDockerGenComponent> Components { get; }
        private Dictionary<IDockerGenComponent, Task> Threads { get; } = new Dictionary<IDockerGenComponent, Task>();

        public DefaultDockerGenRuntime(IEnumerable<IDockerGenComponent> components)
        {
            this.Components = components;
        }

        public async Task Run()
        {
            this.Components.ForEach(component =>
            {
                var task = new Task(async () => { await component.Run(); });
                Threads.Add(component, task);
            });
            
            this.Threads.Values.ForEach(task => task.Start());
            Task.WaitAll(this.Threads.Values.ToArray());
        }
    }
}