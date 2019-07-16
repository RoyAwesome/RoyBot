using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot
{
    public class ReactMenuService
    {
        DiscordSocketClient Client;

        Dictionary<ulong, ReactMenu> Menus = new Dictionary<ulong, ReactMenu>();

        public ReactMenuService(DiscordSocketClient Client)
        {
            this.Client = Client;

            Client.ReactionAdded += ReactionAdded;
            Client.ReactionRemoved += ReactionRemoved;
        }

        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if(Reaction.User.Value.IsBot)
            {
                return Task.CompletedTask;
            }

            ReactMenu Menu;
            if (Menus.TryGetValue(Message.Id, out Menu))
            {
                ReactMenu.OptionInfo Option = Menu.GetOptionByEmoji(Reaction.Emote);
                Option.Released(Client, Channel, Reaction.User.Value);
            }

            return Task.CompletedTask;
        }

        private async Task ReactionAdded(Cacheable<Discord.IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {            
            if (Reaction.User.Value.IsBot)
            {
                return;
            }

            ReactMenu Menu;
            if(Menus.TryGetValue(Message.Id, out Menu))
            {
                ReactMenu.OptionInfo Option = Menu.GetOptionByEmoji(Reaction.Emote);
                await Option.Pressed(Client, Channel, Reaction.User.Value);

                if(Menu.ClearReactAfterPress)
                {
                    var Msg = await Message.GetOrDownloadAsync();
                    await Msg.RemoveReactionAsync(Reaction.Emote, Reaction.User.Value);
                }
            }
        }

        public async Task PostMenu(ReactMenu Menu, IMessageChannel Channel)
        {
            var Message = await Channel.SendMessageAsync(embed: Menu.Embed);


            foreach ( var MenuEntry in Menu.Options)
            {
                await Message.AddReactionAsync(MenuEntry.Button);
            }

            Menu.Guid = new Guid();
            Menu.MessageId = Message.Id;

            Menus.Add(Message.Id, Menu);

        }
    }
}
