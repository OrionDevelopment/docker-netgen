using System;
using System.Collections.Generic;
using docker_netgen.Runtime.Core;

namespace docker_netgen.Configuration
{
    /// <summary>
    /// Represents a single configuration for a generation run.
    /// </summary>
    public class DockerGenConfiguration
    {
        /// <summary>
        /// The template to generate with.
        /// </summary>
        public string Template { get; set; }
        
        /// <summary>
        /// The destination directory into which to generate.
        /// </summary>
        public string Destination { get; set; }
        
        /// <summary>
        /// If the configured generation should watch for changed containers.
        /// </summary>
        public bool Watch { get; set; }
        
        /// <summary>
        /// The amount of time to wait after being triggered to generate.
        /// </summary>
        public TimeSpan Wait { get; set; }
        
        /// <summary>
        /// The command to run after this configuration has run and a change was detected.
        /// </summary>
        public string NotifyCommand { get; set; }
        
        /// <summary>
        /// Indicates if the output of the notify command should be logged.
        /// </summary>
        public bool LogNotifyOutput { get; set; }
        
        /// <summary>
        /// Indicates which commands to send to which containers.
        /// </summary>
        public Dictionary<string, DockerSignal> NotifyContainers { get; set; }
        
        /// <summary>
        /// True to only inspect containers with exposed ports.
        /// </summary>
        public bool OnlyExposed { get; set; }
        
        /// <summary>
        /// True to only inspect containers with published ports. Infers OnlyExposed = true.
        /// </summary>
        public bool OnlyPublished { get; set; }
        
        /// <summary>
        /// True to also inspect stopped containers.
        /// </summary>
        public bool IncludeStopped { get; set; }
        
        /// <summary>
        /// Allows automatic regeneration of the containers after set period of time. Infers Watch = false.
        /// </summary>
        public TimeSpan Interval { get; set; }
        
        /// <summary>
        /// Remove blank lines from the template.
        /// </summary>
        public bool KeepBlankLines { get; set; }
    }
}