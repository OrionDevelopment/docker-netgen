using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using docker_netgen.Configuration;
using docker_netgen.Runtime;
using docker_netgen.Runtime.Component;
using docker_netgen.Runtime.Core;
using docker_netgen.Template;
using docker_netgen.Template.Core;
using docker_netgen.Template.Parsing;
using docker_netgen.Utils;
using Docker.DotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace docker_netgen
{
    public class Startup
    {

        public static void ConfigureServices(IServiceCollection services, string[] args)
        {
            ConfigureLoggingServices(services);
            ConfigureConfiguration(services, args);
            ConfigureDockerClientServices(services);
            ConfigureDockerGenConfigurations(services);
            ConfigureDockerGenRuntime(services);
            ConfigureTemplateEngine(services);
        }

        private static void ConfigureLoggingServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
        }
        
        private static void ConfigureConfiguration(IServiceCollection services, string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dockergen.json", false, true)
                .AddJsonFile("configurations.json", true, false)
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
        }

        private static void ConfigureDockerClientServices(IServiceCollection services)
        {
            services.AddSingleton<DockerClientConfiguration>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var dockerSection = configuration.GetSection("docker");

                return new DockerClientConfiguration(
                    dockerSection.GetValue("endpoint", DockerUtils.getDefaultDockerEndpoint()),
                    new AnonymousCredentials(),
                    dockerSection.GetValue("timeout", TimeSpan.Zero));
            });

            services.AddTransient<IDockerClient>(provider =>
            {
                var dockerClientConfiguration = provider.GetService<DockerClientConfiguration>();
                return dockerClientConfiguration.CreateClient();
            });
        }

        private static void ConfigureDockerGenConfigurations(IServiceCollection services)
        {
            services.AddSingleton<IEnumerable<DockerGenConfiguration>>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var configurationsSection = configuration.GetSection("configuration");

                var dockerGenConfigurations = new List<DockerGenConfiguration>();
                foreach (var configurationSection in configurationsSection.GetChildren())
                {
                    var dockerGenConfiguration = new DockerGenConfiguration();
                    configurationSection.Bind(dockerGenConfiguration);
                    dockerGenConfigurations.Add(dockerGenConfiguration);
                }

                if (dockerGenConfigurations.Any()) return dockerGenConfigurations;
                
                var localConfiguration = new DockerGenConfiguration();
                configuration.Bind(localConfiguration);
                dockerGenConfigurations.Add(localConfiguration);

                return dockerGenConfigurations;
            });
        }

        private static void ConfigureDockerGenRuntime(IServiceCollection services)
        {
            services.AddSingleton<IDockerGenRuntime, DefaultDockerGenRuntime>();
            services.AddSingleton<IDockerGenComponent, SingleRunContainerBasedComponent>();
            services.AddSingleton<IDockerGenComponent, TimerBasedTriggeringComponent>();
            services.AddSingleton<IDockerGenComponent, EventWatchingComponent>();
        }

        private static void ConfigureTemplateEngine(IServiceCollection services)
        {
            services.AddSingleton<ITemplateFactory, SimpleTemplateFactory>();
            services.Decorate<ITemplateFactory, CachingTemplateFactory>();
            services.AddSingleton<IImportParser, SimpleImportParser>();
        }
    }
}