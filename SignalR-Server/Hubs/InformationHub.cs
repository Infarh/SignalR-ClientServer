using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SignalR_Server.Hubs
{
    public class InformationHub : Hub
    {
        private readonly ILogger<InformationHub> _Logger;

        public InformationHub(ILogger<InformationHub> Logger) => _Logger = Logger;

        public async Task ServerMessage(string User, string Message)
        {
            var current_time = DateTime.Now;
            _Logger.LogInformation("Message from {0} at {1:dd.MM.yy HH:mm:ss}: {2}", User, current_time, Message);
            await Clients.All.SendAsync("ClientMessage", current_time, User, Message);
        }

        public async Task IntroduceClient(string UserName)
        {
            var current_time = DateTime.Now;
            _Logger.LogInformation("Новый клиент с именем: {0}[{1:dd.MM.yy HH:mm:ss}]", UserName, current_time);
            await Clients.All.SendAsync("IntroduceNewClient", current_time, UserName);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _Logger.LogInformation("Client connected");
            await Clients.All.SendAsync("ClientConnected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            _Logger.LogInformation("Client disconnected");
            await Clients.All.SendAsync("ClientDisconnected");
        }
    }
}
