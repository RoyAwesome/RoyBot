using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace RoyBot.Server
{
    public class ServerNotSetup : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            ServerDatabase ServerDb = services.GetService<ServerDatabase>();

            if(ServerDb.Servers.FirstOrDefault(x => x.ServerSnowflake == context.Guild.Id) != null)
            {
                return Task.FromResult(PreconditionResult.FromError("This server is already setup!"));
            }

            return Task.FromResult(PreconditionResult.FromSuccess()); 

        }
    }
}
