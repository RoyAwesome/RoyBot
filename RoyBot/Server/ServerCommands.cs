using Discord.Commands;
using Discord.WebSocket;
using RoyBot.Attributes;
using RoyBot.InteractionProcess;
using RoyBot.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot.Server
{
    [Group("server")]
    public class ServerCommands : ModuleBase<SocketCommandContext>
    {
        public InteractProcessService InteractProcess
        {
            get;
            set;
        }

        private readonly ServerDatabase ServerDb;

        public ServerCommands(InteractProcessService IPS, ServerDatabase ServerDb)
        {
            InteractProcess = IPS;
            this.ServerDb = ServerDb;
        } 
        
        [Command("purge")]
        [RequireContext(ContextType.Guild)]
        [ServerSetupCompleted]
        [BotOwner]
        public async Task Purge()
        {
            InteractProcessBuilder IPB = new InteractProcessBuilder("Are you sure?")
                .WhenSuccessful(async (InteractProcess Process) =>
                {
                    ServerModel m = ServerDb.Servers.First(x => x.ServerSnowflake == Context.Guild.Id);
                    ServerDb.Remove(m);
                    ServerDb.SaveChangesAsync();
                    await Process.Channel.SendMessageAsync("Purged");
                })
                .WhenCancelled(async (InteractProcess Process) => await Process.Channel.SendMessageAsync("Cancelled"))
                .WithStep("Are you sure? (say `cancel` to cancel)");


            await InteractProcess.BeginInteractionProcess(IPB.Build(), Context.User, Context.Channel);
        }

        [Command("setup")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [ServerNotSetup]
        public async Task Setup()
        {           
            ServerModel ServerDataObject = new ServerModel();
            ServerDataObject.ServerGUID = Guid.NewGuid();
            ServerDataObject.Owner = Context.Guild.OwnerId;
            ServerDataObject.ServerSnowflake = Context.Guild.Id;
                      

            InteractProcessBuilder IPB = new InteractProcessBuilder("Server Configure")
            .WithUserObject(ServerDataObject)
            .WithDefaultStepCallback(async (SocketMessage Message, InteractProcess Process, InteractProcess.ProcessStep Step) =>
            {              
                
                typeof(ServerModel).GetProperty(Step.UserObject as string).SetValue(Process.UserObject, Message.Content);
            })
            .WhenSuccessful(async (InteractProcess Process) =>
            {
                ServerDb.Servers.Add(Process.UserObject as ServerModel);
                ServerDb.SaveChangesAsync();
                await Process.Channel.SendMessageAsync("Done.  Your server is configured");
            })
            .WithStep("Describe your server", StepUserObject: nameof(ServerModel.Description))
            .WithStep("Ping your administrator roles (this role will be able to use admin commands, and you can mention multiple roles)"
                , Callback: async (SocketMessage Message, InteractProcess Process, InteractProcess.ProcessStep Step) =>
                {
                    ServerModel Model = Process.UserObject as ServerModel;
                    Model.ManagementRoles = Message.MentionedRoles.Select(x => new ManagementRole { Role = x.Id }).ToList();
                })
            .WithStep("What is a channel I can post to for admin related messages?", 
            Callback: async (SocketMessage Message, InteractProcess Process, InteractProcess.ProcessStep Step) =>
            {                
                if(Message.MentionedChannels.Count == 0)
                {
                    (Process.UserObject as ServerModel).AdminChannel = Message.Channel.Id;
                }
                (Process.UserObject as ServerModel).AdminChannel = Message.MentionedChannels.FirstOrDefault().Id;
            });

            await InteractProcess.BeginInteractionProcess(IPB.Build(), Context.User, Context.Channel);
        }

        [Command("config")]
        [RequireContext(ContextType.Guild)]
        [ServerSetupCompleted]
        public async Task Config([ValidPropertyName(typeof(ServerModel))] string property, string value)
        {
            ServerModel m = ServerDb.Servers.First(x => x.ServerSnowflake == Context.Guild.Id);

            typeof(ServerModel).GetProperty(property).SetValue(m, value);

            ServerDb.Update(m);

            await ServerDb.SaveChangesAsync();
        }

        [Command("config")]
        [RequireContext(ContextType.Guild)]
        [ServerSetupCompleted]
        public async Task Config()
        {
            string ListOfProps = string.Join(',', typeof(ServerModel).GetProperties().Where(x => x.GetCustomAttribute<UserEditableAttribute>() != null).Select(x => x.Name));
            await ReplyAsync("Editable Properties: [" + ListOfProps + "]");
        }

        [Command("config")]
        [RequireContext(ContextType.Guild)]
        [ServerSetupCompleted]
        public async Task Config([ValidPropertyName(typeof(ServerModel))] string property)
        {
            ServerModel m = ServerDb.Servers.First(x => x.ServerSnowflake == Context.Guild.Id);
            var Value = typeof(ServerModel).GetProperty(property).GetValue(m);

            await ReplyAsync(string.Format("The Value of {0} is `{1}`", property, Value.ToString()));
        }
    }
}
