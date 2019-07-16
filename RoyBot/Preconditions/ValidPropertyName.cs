using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using RoyBot.Attributes;

namespace RoyBot
{
    public class ValidPropertyName : ParameterPreconditionAttribute
    {
        private Type Type;
        public ValidPropertyName(System.Type Type)
        {
            this.Type = Type;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, Discord.Commands.ParameterInfo parameter, object value, IServiceProvider services)
        {
            PropertyInfo Property = Type.GetProperty(value as string);
            if (Property == null || Property.GetCustomAttribute<UserEditableAttribute>() == null)
            {
                return Task.FromResult(PreconditionResult.FromError(
                    string.Format("That is an invalid property.  Valid properties are: [{0}]",
                    string.Join(',', Type.GetProperties().Where(x => x.GetCustomAttribute<UserEditableAttribute>() != null).Select(x => x.Name))) ));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
