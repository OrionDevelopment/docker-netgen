using System;
using System.Runtime.InteropServices;

namespace docker_netgen.Utils
{
    /// <summary>
    /// Provides general utilities for handling interactions with the docker client.
    /// </summary>
    public static class DockerUtils
    {

        /// <summary>
        /// Returns the default docker client endpoint, based on the OS.
        /// Windows uses NPipes, while Unix based systems use a Socket.
        /// </summary>
        /// <returns>The default hardcoded docker client endpoint, based on the os.</returns>
        public static Uri getDefaultDockerEndpoint()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
        }
    }
}