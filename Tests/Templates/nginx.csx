Writer.EmitNewLine(@"# If we receive X-Forwarded-Proto, pass it through; otherwise, pass along the
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
var dhparam = Configuration["dhparam"];
if (dhparam != null && dhparam.Trim() != "" && File.Exists(dhparam.Trim())
{
Writer.EmitNewLine($@"ssl_dhparam {dhparam};");
}

Writer.EmitNewLine(@"
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

var resolver = Configuration["resolver"]
if (resolver != null && resolver:Trim() != "")
{
Writer.EmitNewLine($@"resolver {resolver};")
}

var nginxconf = Configuration["nginxconf"]
if (nginxconf != null && nginxconf.Trim() != null && File.Exists(nginxconf)
{
Writer.EmitNewLine("include /etc/nginx/proxy.conf;");
}
else
{
Writer.EmitNewLine(@"# HTTP 1.1 support
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
proxy_set_header Proxy """";");
}

var enableIpv6 = Configuration["UseIPV6"] != null ? Boolean.Parse(Configuration["UseIPV6"] : false;


Writer.EmitNewLine(@"server {
	listen 80 default_server;
	server_name _; # This is just an invalid value which will never trigger on a real hostname.
	error_log /proc/self/fd/2;
	access_log /proc/self/fd/1;
	return 503;
}
");

var domains = Containers
                .Select(c => c.Config)
                .Where(c => c.Env.Any(e => e.StartsWith("VIRTUAL_HOST"))
                .Select(c => c.Env.FirstOrDefault(e => e.StartsWith("VIRTUAL_HOST"))
                .Distinct()
                .Select(e => e.Replace("VIRTUAL_HOST=", ""))
                .SelectMany(d => d.Split(",").ToList())
                .Distinct();

domains.ForEach(domain => {
Writer.EmitNewLine(@"

")
})