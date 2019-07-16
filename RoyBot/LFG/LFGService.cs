using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoyBot.LFG
{
    public class LFGService
    {
        private readonly DiscordSocketClient Client;

        public LFGService(DiscordSocketClient Client)
        {
            this.Client = Client;

           
        }
    }
}
