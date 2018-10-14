//Static import for the output utilities
using static docker_netgen.Template.Script.ScriptOutput;

//Normal imports for all the classes
//Limited to docker-netgens references
//Normal reference loading by CSharpScript still supported (see #r)
using docker_netgen.Template.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using docker_netgen.Utils;
using Docker.DotNet.Models;

//Default configuration for a reverse proxy
EmitNewLine(Writer, @"# If we receive X-Forwarded-Proto, pass it through; otherwise, pass along the
# scheme used to connect to this server
map $http_x_forwarded_proto $proxy_x_forwarded_proto {
  default $http_x_forwarded_proto;
  ''      $scheme;
}

# If we receive X-Forwarded-Port, pass it through; otherwise, pass along the
# server port the client connected to
map $http_x_forwarded_port $proxy_x_forwarded_port {
  default $http_x_forwarded_port;
  ''      $server_port;
}

# If we receive Upgrade, set Connection to ""upgrade""; otherwise, delete any
# Connection header that may have been passed to this server
map $http_upgrade $proxy_connection {
  default upgrade;
  '' close;
}

# Apply fix for very long server names
server_names_hash_bucket_size 128;

# Default dhparam");
//Setup the default diffy hellmann parameters.
var dhparam = Configuration["DHParam"];
if (dhparam != null && dhparam.Trim() != "" && File.Exists(dhparam.Trim()))
{
EmitNewLine(Writer, $@"ssl_dhparam {dhparam};");
}

//Setup appropriate SSL Forwarding headers as well as compression and logging
EmitNewLine(Writer, @"
# Set appropriate X-Forwarded-Ssl header
map $scheme $proxy_x_forwarded_ssl {
  default off;
  https on;
}

gzip_types text/plain text/css application/javascript application/json application/x-javascript text/xml application/xml application/xml+rss text/javascript;

log_format vhost '$host $remote_addr - $remote_user [$time_local] '
                 '""$request"" $status $body_bytes_sent '
                 '""$http_referer"" ""$http_user_agent""';

access_log off;
");

//Check if a resolver is defined.
var resolver = Configuration["Resolver"];
//Load a custom Resolver.
if (resolver != null && resolver.Trim() != "")
{
EmitNewLine(Writer, $@"resolver {resolver};");
}

//Check if the user has his own Nginx configuration specified.
var nginxConf = Configuration["NginxConf"];

//If the users nginxconf exist then load that.
if (nginxConf != null && nginxConf.Trim() != null && File.Exists(nginxConf))
{
EmitNewLine(Writer, $"include {nginxConf}");
}
//If not load the default configuration.
else
{
EmitNewLine(Writer, @"# HTTP 1.1 support
proxy_http_version 1.1;
proxy_buffering off;
proxy_set_header Host $http_host;
proxy_set_header Upgrade $http_upgrade;
proxy_set_header Connection $proxy_connection;
proxy_set_header X-Real-IP $remote_addr;
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $proxy_x_forwarded_proto;
proxy_set_header X-Forwarded-Ssl $proxy_x_forwarded_ssl;
proxy_set_header X-Forwarded-Port $proxy_x_forwarded_port;

# Mitigate httpoxy attack (see README for details)
proxy_set_header Proxy """";
");
}

//Generate the default server section (triggered when no container matches the domain)
EmitNewLine(Writer, @"server {
	server_name _; # This is just an invalid value which will never trigger on a real hostname.
	listen 80;");

//Check if IPV6 is enabled and if so bind to all IPV6 ips on port 80.
var enableIpv6 = Configuration["UseIPV6"] != null ? Configuration.GetSection("UseIPV6").Get<bool>() : false;
if (enableIpv6)
{
EmitNewLine(Writer, "	listen [::]:80;");
}

//Emit the remaining section of the default server part.
EmitNewLine(Writer, @"    access_log /var/log/nginx/access.log vhost;
	return 503;
}");

//Check if we use a single default certificate for all containers.
var defaultCertFile = Configuration["DefaultCert"];
var defaultCertKeyFile = Configuration["DefaultKey"];

//If a certificate is set then generate a SSL secured endpoint with the specified certificate and keyfile.
if (defaultCertFile != null && defaultCertKeyFile != null && File.Exists(defaultCertFile) && File.Exists(defaultCertKeyFile))
{
EmitNewLine(Writer, @"server {
	server_name _; # This is just an invalid value which will never trigger on a real hostname.
	listen 443 ssl http2;");

//Check if IPV6 is enabled.
if (enableIpv6)
{
EmitNewLine(Writer, "	listen [::]:80;");
}

//Emit the remaining configuration for the default host when SSL is in use.
EmitNewLine(Writer, $@"    access_log /var/log/nginx/access.log vhost;
	return 503;

	ssl_session_tickets off;
	ssl_certificate {defaultCertFile};
	ssl_certificate_key {defaultCertKeyFile};
}}");
}

//Holds the current container
var currentContainer = Containers.First(c => c.ID.Equals(DockerUtils.GetCurrentContainerId()));

//Group all domains together based on the virtual host key.
var groupedContainersByDomain = Containers
                    .Where(c => c.Config.Env.Any(e => e.StartsWith("VIRTUAL_HOST")))
                    .GroupBy(c => c.Config.Env.FirstOrDefault(s => s.StartsWith("VIRTUAL_HOST="))?.Replace("VIRTUAL_HOST=", "").Trim())
                    .ToList();

//Loop over the containers and create upstream references from their input.
foreach (IGrouping<string,ContainerInspectResponse> grouping in groupedContainersByDomain)
{
    //Skip all groupings whos key is empty or null (No virtual host specified)
    if (grouping.Key == null || !grouping.Key.Any())
    {
        continue;
    }

    //The domain cna be hashed if need be.
    var domain = grouping.Key;
    domain = domain.StartsWith("~") ? domain.Sha1() : domain;

    //Comment with the domain and write the upstream start tag.
    EmitNewLine(Writer, $@"# {domain}
upstream {domain} {{");

    //Now loop over the containers in the responses and create their relevant server tags.
    foreach (ContainerInspectResponse response in grouping)
    {
        //Loop over all connected networks
        foreach (string currentContainerNetworkName in currentContainer.NetworkSettings.Networks.Keys)
        {
            //Loop over all networks the target container is connected too
            foreach (string containerNetworkName in response.NetworkSettings.Networks.Keys)
            {
                if (containerNetworkName != "ingress" &&
                    (currentContainerNetworkName == containerNetworkName) || containerNetworkName == "host")
                {
                    var openPortCount = response.NetworkSettings.Ports.Count;
                    var exposedPortCount = response.Config.ExposedPorts.Count;
                    
                    PortBinding portBinding = null;
                    var exposedPort = -1;

                    if (openPortCount == 1 || (openPortCount == 0 && exposedPortCount == 1))
                    {
                        if (openPortCount == 1)
                        {
                            portBinding = response.NetworkSettings.Ports.First().Value[0];
                            exposedPort = Convert.ToInt32(portBinding.HostPort);
                        }
                        else
                        {
                            exposedPort =
                                Convert.ToInt32(response.Config.ExposedPorts.First().Key.Split("/")[0]);
                        }
                    }
                    else
                    {
                        exposedPort = Convert.ToInt32(response.Config.Env.First(e => e.StartsWith("VIRTUAL_PORT="))
                            .Replace("VIRTUAL_PORT=", ""));

                        portBinding = new PortBinding()
                        {
                            HostIP = response.NetworkSettings.Networks[containerNetworkName].IPAddress,
                            HostPort = exposedPort.ToString()
                        };
                    }
                        
                    GenerateServerSection(response, portBinding, response.NetworkSettings.Networks[containerNetworkName], exposedPort);
                }
                else
                {
                    EmitNewLine(Writer, @"# Cannot connect to network of this container
                    server 127.0.0.1 down;"); 
                }
            }
        }
    }

    //Emit the closing tag for the domain.
    EmitNewLine(Writer, "}");
}
        
void GenerateServerSection(ContainerInspectResponse container, PortBinding portBinding, EndpointSettings network, int port)
{
    if (portBinding != null)
    {
        if (container.Node != null)
        {
            //Container is connected to a swarm node with exposed port. Connecting via swarm.
            EmitNewLine(Writer, $"# {container.Node.Name}/{container.Name}");
            EmitNewLine(Writer, $"server {container.Node.IPAddress}:{port};");
        }
        else
        {
            //Container is connected via local docker network and port.
            EmitNewLine(Writer, $"# container.Name");
            EmitNewLine(Writer, $"server {network.IPAddress}:{port};");
        }
    }
    else if (port >= 0)
    {
        //Container is connected via local docker network and port.
        EmitNewLine(Writer, $"# container.Name");
        EmitNewLine(Writer, $"server {network.IPAddress}:{port};");
    }
    else
    {
        //Container is connected via local docker network.
        EmitNewLine(Writer, $"# container.Name");
        EmitNewLine(Writer, $"server {network.IPAddress} down;");
    }
}

/*
var domains = Containers
                .Select(c => c.Config)
                .Where(c => c.Env.Any(e => e.StartsWith("VIRTUAL_HOST"))
                .Select(c => c.Env.FirstOrDefault(e => e.StartsWith("VIRTUAL_HOST"))
                .Distinct()
                .Select(e => e.Replace("VIRTUAL_HOST=", ""))
                .SelectMany(d => d.Split(",").ToList())
                .Distinct();

domains.ForEach(domain => {
EmitNewLine(@"

")
})
*/