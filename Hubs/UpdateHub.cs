using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Stratis.Guru.Hubs
{
    public class UpdateHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}