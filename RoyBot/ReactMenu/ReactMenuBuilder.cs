using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoyBot
{
    public class ReactMenuBuilder
    {
        private ReactMenu Menu;
        private Embed Embed;
        private string Title;
        private string Description;
        private IUser User;

        private ReactMenu.ButtonCallback DefaultPressedCallback;
        private ReactMenu.ButtonCallback DefaultReleasedCallback;




        public ReactMenuBuilder()
        {
            Menu = new ReactMenu();
            Embed = null;
        }

        public ReactMenu Build()
        {
            if(Embed == null)
            {
                Embed = BuildDefaultEmbed();
            }
            Menu.Embed = Embed;

            return Menu;
        }
        public ReactMenuBuilder WithTitle(string Title)
        {
            this.Title = Title;
            return this;
        }

        public ReactMenuBuilder WithDescription(string Description)
        {
            this.Description = Description;
            return this;
        }

        public ReactMenuBuilder FromUser(IUser User)
        {
            this.User = User;
            return this;
        }

        public ReactMenuBuilder WithDefaultPressedCallback(ReactMenu.ButtonCallback OnPressed)
        {
            DefaultPressedCallback = OnPressed;
            return this;
        }

        public ReactMenuBuilder WithDefaultReleasedCallback(ReactMenu.ButtonCallback OnReleased)
        {
            DefaultReleasedCallback = OnReleased;
            return this;
        }

        public ReactMenuBuilder WithOption(string Description, IEmote ButtonEmote, ReactMenu.ButtonCallback OnPressed = null, ReactMenu.ButtonCallback OnReleased = null)
        {
            ReactMenu.OptionInfo Option = new ReactMenu.OptionInfo
            {
                Description = Description,
                Button = ButtonEmote
            };
            if(OnPressed != null)
            {
                Option.PressedCallback += OnPressed;
            } 
            else if(DefaultPressedCallback != null)
            {
                Option.PressedCallback += DefaultPressedCallback;
            }
           
            if(OnReleased != null)
            {
                Option.ReleasedCallback += OnReleased;
            }
            else if(DefaultReleasedCallback != null)
            {
                Option.ReleasedCallback += DefaultReleasedCallback;
            }

            Menu.Options.Add(Option);

            return this;
        }

        public ReactMenuBuilder WithCustomEmbed(Embed Embed)
        {
            this.Embed = Embed;
            return this;
        }

        public ReactMenuBuilder WithClearReactAfterPress(bool ClearReact)
        {
            Menu.ClearReactAfterPress = ClearReact;
            return this;
        }

        protected Embed BuildDefaultEmbed()
        {
            List<EmbedFieldBuilder> Fields = new List<EmbedFieldBuilder>();
            foreach(ReactMenu.OptionInfo o in Menu.Options)
            {
                EmbedFieldBuilder FB = new EmbedFieldBuilder();
                FB.WithName(o.Button.ToString())
                    .WithValue(o.Description);
               

                Fields.Add(FB);
            }

            EmbedBuilder Builder = new EmbedBuilder();
            Builder.WithTitle(Title)
                .WithFields(Fields);

            if(User != null)
            {
                Builder.WithAuthor(User);
            }
            return Builder.Build();
        }

    }
}
