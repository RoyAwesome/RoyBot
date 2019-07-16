using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RoyBot.Properties;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoyBot.InteractionProcess;
using RoyBot.Server;

namespace RoyBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        DiscordSocketClient Client;
        IServiceProvider ServiceProvider;

        public async Task MainAsync()
        {           
            ConfigureLogger();
            ServiceProvider = ConfigureServices();

            await ServiceProvider.GetRequiredService<Server.ServerDatabase>().Database.EnsureCreatedAsync();
            await ServiceProvider.GetRequiredService<CommandHandler>().InstallCommandsAsync();


            Client.Log += Log;
            //Client.MessageReceived += MessageRecieved;

            await Client.LoginAsync(TokenType.Bot, Properties.BotSettings.Default.DiscordToken);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            Client = new DiscordSocketClient();
            return new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton<ServerService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<ReactMenuService>()
                .AddSingleton<InteractProcessService>()                
                .AddDbContext<Server.ServerDatabase>(options => options.UseSqlite("DataSource=mydatabase.db;"))                
                //.AddLogging()
                .BuildServiceProvider();
        }

        NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Task Log(LogMessage Message)
        {
            Logger.Info(Message.ToString());
            return Task.CompletedTask;
        }

      

        private async Task MessageRecieved(SocketMessage Message)
        {
            if(Message.Content == "!ping")
            {
                await Message.Channel.SendMessageAsync("Pong");
            }
        }

        private void ConfigureLogger()
        {
            var Config = new NLog.Config.LoggingConfiguration();
            var LogConsole = new NLog.Targets.ConsoleTarget("LogConsole");
            var FileTarget = new NLog.Targets.FileTarget("File")
            {
                FileName = "${basedir}/file.txt",
                Layout = "${message}"
            };

            Config.AddRuleForAllLevels(LogConsole);
            Config.AddRuleForAllLevels(FileTarget);

            NLog.LogManager.Configuration = Config;

        }
    }
}
