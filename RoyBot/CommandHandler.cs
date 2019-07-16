using Discord.Commands;
using Discord.Commands.Builders;
using Discord.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot
{
    class CommandHandler
    {
        Logger Log = LogManager.GetCurrentClassLogger();


        private readonly DiscordSocketClient Client;
        private readonly CommandService Commands;
        private readonly IServiceProvider ServiceProvider;

        public CommandHandler(DiscordSocketClient Client, CommandService CommandServ, IServiceProvider ServiceProvider)
        {
            this.Client = Client;
            this.Commands = CommandServ;
            this.ServiceProvider = ServiceProvider;
        }

        public async Task InstallCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), 
                                        services: ServiceProvider);
        }
       
        private async Task HandleCommandAsync(SocketMessage Message)
        {

            var m = Message as SocketUserMessage;
            if(m == null)
            {
                return;
            }

            int argpos = 0;

            if(! (m.HasCharPrefix('!', ref argpos) ||
                m.HasMentionPrefix(Client.CurrentUser, ref argpos) ||
                Message.Author.IsBot))
            {
                Log.Debug(Message.ToString());
                return;
            }

            var Context = new SocketCommandContext(Client, m);

            var Result = await Commands.ExecuteAsync(Context, argpos, ServiceProvider);

            if(Result.Error != null && Result.Error != CommandError.UnknownCommand)
            {
                Log.Info(string.Format("Command {0} from {1} on server {3} failed: {2}", Context.Message.Content, Context.User.ToString(), Result.ErrorReason, Context.Guild.Name));

                bool MessageError = false;

                //Make sure that the user has some permissions, or we'll not spam error messages
                SocketGuildUser User = Message.Author as SocketGuildUser;
                if (User != null)
                {
                    MessageError |= User.GuildPermissions.RawValue > 0;
                }

                if (!Result.IsSuccess && MessageError)
                {
                    await Message.Channel.SendMessageAsync("Error: " + Result.ErrorReason);
                }
            }

        }
    }
}
