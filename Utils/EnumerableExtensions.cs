using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace docker_netgen.Utils
{
    public static class EnumerableExtensions
    {

        public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> action)
        {
            foreach (var e in collection)
            {
                await Task.Run(async () => { await action(e); });
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var e in collection)
            {
                action(e);
            }
        }
    }
}