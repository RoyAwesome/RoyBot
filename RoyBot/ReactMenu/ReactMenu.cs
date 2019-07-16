using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RoyBot
{
    public class ReactMenu
    {
        public delegate Task ButtonCallback(DiscordSocketClient Client, ISocketMessageChannel Channel, IUser User, bool Pressed, OptionInfo OptionInfo);

        public Guid Guid
        {
            get;
            set;
        }

        public ulong MessageId
        {
            get;
            set;
        }

        public struct OptionInfo
        {
            public string Description;
            public IEmote Button;
            public event ButtonCallback PressedCallback;
            public event ButtonCallback ReleasedCallback;


            public Task Pressed(DiscordSocketClient Client, ISocketMessageChannel Channel, IUser User)
            {
                if(PressedCallback == null)
                {
                    return Task.CompletedTask;
                }
                return PressedCallback.Invoke(Client, Channel, User, true, this);
            }

            public Task Released(DiscordSocketClient Client, ISocketMessageChannel Channel, IUser User)
            {
                if (ReleasedCallback == null)
                {
                    return Task.CompletedTask;
                }
                return ReleasedCallback.Invoke(Client, Channel, User, false, this);
            }
        }
        
        public Embed Embed
        {
            get;
            set;
        }

        public List<OptionInfo> Options
        {
            get;
            set;
        } 
        
        public bool ClearReactAfterPress
        {
            get;
            set;
        }
       

        public ReactMenu()
        {
            Options = new List<OptionInfo>();
        }


        public OptionInfo GetOptionByEmoji(IEmote Emoji)
        {
            foreach(var opt in Options)
            {
                if(opt.Button.ToString() == Emoji.ToString())
                {
                    return opt;
                }
            }
            return new OptionInfo();
        }

    }
}
