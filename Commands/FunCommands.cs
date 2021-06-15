using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns \"Ping!\"")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync("Pong!").ConfigureAwait(false);
        }
    }
}