using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SignalR_Server.Hubs;

namespace SignalR_Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services
           .AddSignalR();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app
           .UseRouting()
           .UseEndpoints(endpoints => endpoints.MapHub<InformationHub>("/information"));
    }
}
