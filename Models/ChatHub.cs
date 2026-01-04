using Microsoft.AspNetCore.SignalR;

namespace CVBuddy.Models
{
    public class ChatHub : Hub
    {

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("RecieveMessage", user, message);
        }

    }
}
