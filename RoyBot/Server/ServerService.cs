using Discord.WebSocket;
using RoyBot.InteractionProcess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot.Server
{  
    public class ServerService
    {
        private readonly DiscordSocketClient Client;

        public ServerService(DiscordSocketClient Client)
        {
            this.Client = Client;

            Client.JoinedGuild += OnJoinedGuild;
          
        }

        private async Task OnJoinedGuild(SocketGuild Guild)
        {
            await Guild.DefaultChannel.SendMessageAsync("Hello! I am Roybot!  If you want to use me, the server owner should do !server setup");
        }
    }
}
