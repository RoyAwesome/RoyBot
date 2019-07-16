using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RoyBot.InteractionProcess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot.LFG
{
    [Group("lfg")]
    public class LFGCommands : ModuleBase<SocketCommandContext>
    {
        ReactMenuService MenuService;

        public InteractProcessService InteractProcess
        {
            get;
            set;
        }

        LFGCommands(ReactMenuService RMS, InteractProcessService IPS)
        {
            MenuService = RMS;
            InteractProcess = IPS;
        }

        [Group("admin")]
        public class LFGAdmin : ModuleBase<SocketCommandContext>
        {
            public InteractProcessService InteractProcess
            {
                get;
                set;
            }

            [Command("init")]
            [RequireContext(ContextType.Guild)]
            [RequireBotPermission(GuildPermission.ManageRoles | GuildPermission.ManageChannels, ErrorMessage = "I need to be able to manager roles and channels to do LFG things")]
            public async Task Init()
            {
                InteractProcessBuilder IPB = new InteractProcessBuilder("LFG Configure")
                    .WithStep("What Category should the LFG channels be created under? (right click on a category and copy it's ID. say `create one` and I'll do it for you")
                    .WithStep("What Role should I base the group role off of? (mention it)")
                    .WithStep("Should I create a text channel for each group? (reply with `yes` or `no`)")
                    .WithStep("Should I create a voice channel for each group? (reply with `yes` or `no`)")
                    .WithStep("If a non group user joins a group channel, should I automatically add them to the group? (reply with `yes` or `no`)")
                    ;

                await InteractProcess.BeginInteractionProcess(IPB.Build(), Context.User, Context.Channel);
            }

        }



        [Command("list")]
        public async Task List()
        {
            var MB = new ReactMenuBuilder()
                .WithTitle("Test")
                .WithDescription("THis is a description")
                .WithDefaultPressedCallback(async (DiscordSocketClient Client, ISocketMessageChannel Channel, IUser User, bool bPressed, ReactMenu.OptionInfo OptionInfo) =>
                {
                    await Channel.SendMessageAsync("Pressed!");
                })
                .WithDefaultReleasedCallback(async (DiscordSocketClient Client, ISocketMessageChannel Channel, IUser User, bool bPressed, ReactMenu.OptionInfo OptionInfo) =>
                {
                    await Channel.SendMessageAsync("Released!");
                })
                .WithOption("Test One", EmojiExt.One)
                .WithOption("Test Two", EmojiExt.Two)
                .WithClearReactAfterPress(true);

            await MenuService.PostMenu(MB.Build(), Context.Channel);
        }

    }
}
