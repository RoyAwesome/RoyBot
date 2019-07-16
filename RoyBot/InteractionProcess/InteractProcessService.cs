using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot.InteractionProcess
{
    public class InteractProcessService
    {
        DiscordSocketClient Client;

        List<InteractProcess> ActiveInteractProcesses = new List<InteractProcess>();

        public InteractProcessService(DiscordSocketClient Client)
        {
            this.Client = Client;

            Client.MessageReceived += OnMessageRecieved;
        }

        private async Task OnMessageRecieved(SocketMessage arg)
        {
            if(arg.Author.IsBot)
            {
                return;
            }
            var m = arg as SocketUserMessage;
            int argpos = 0;
            //Skip commands
            if(m.HasCharPrefix('!', ref argpos))
            {
                return;
            }
                      

            InteractProcess Process = GetInteractProcessForUser(arg.Author);
            if(Process != null)
            {
                if(arg.Content.ToLower() == "cancel")
                {
                    Process.Cancel();
                }
                else
                {
                    bool MoreSteps = Process.ExecuteStep(arg);
                    if (MoreSteps)
                    {
                        await PromptUserForStep(Process);
                    }
                }                
            }
                                 
        }

        public Task BeginInteractionProcess(InteractProcess Process, IUser User, ISocketMessageChannel Channel)
        {
            Process.InitiatingUser = User;
            Process.Channel = Channel;
            Process.Client = Client;
            Process.OnProcessCancel += EndProcess;
            Process.OnProcessComplete += EndProcess;
            Process.OnProcessStart += ProcessStart;

            Process.Start();

            return Task.CompletedTask;
        }       

        private async Task ProcessStart(InteractProcess Process)
        {
            ActiveInteractProcesses.Add(Process);
            await PromptUserForStep(Process);
        }

        private Task EndProcess(InteractProcess Process)
        {
            ActiveInteractProcesses.Remove(Process);
            return Task.CompletedTask;
        }

        private async Task PromptUserForStep(InteractProcess Process)
        {
            await Process.Channel.SendMessageAsync(Process.CurrentStep.Description);
        }

        private InteractProcess GetInteractProcessForUser(IUser Speaker)
        {
            foreach(var Process in ActiveInteractProcesses)
            {
                if(Process.InitiatingUser == Speaker)
                {
                    return Process;
                }    
            }
            return null;
        }
    }

}
