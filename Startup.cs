using System;
using System.Collections.Generic;
using docker_netgen.Configuration;
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
                .AddJsonFile("dockergen.json", false, true)
                .AddJsonFile("configurations.json", true, false);
            
            services.AddSingleton(configuration);
        }

        private static void ConfigureDockerClientServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var dockerSection = configuration.GetSection("docker");

                return new DockerClientConfiguration(
                    dockerSection.GetValue("endpoint", DockerUtils.getDefaultDockerEndpoint()),
                    dockerSection.GetValue("credentials", new AnonymousCredentials()),
                    dockerSection.GetValue("timeout", TimeSpan.Zero));
            });

            services.AddTransient(provider =>
            {
                var dockerClientConfiguration = provider.GetService<DockerClientConfiguration>();
                return dockerClientConfiguration.CreateClient();
            });
        }

        private static void ConfigureDockerGenConfigurations(IServiceCollection services)
        {
            services.AddSingleton(provider =>
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
                
                if (dockerGenConfigurations.)
                
            })
        }
    }
}