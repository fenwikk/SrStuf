using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Commands
{
    public class UtilityCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns \"Pong!\"")]
        public async Task Ping(CommandContext ctx)
        {
            var pong = await ctx.RespondAsync("Pong!");

            var latency = (DateTime.UtcNow - ctx.Message.CreationTimestamp).ToString("fff");

            await pong.DeleteAsync();

            var embed = Bot.CreateEmbed(ctx);

            embed.Title = "Pong!";
            embed.AddField("Latency", "```ini\n[ " + latency + "ms ]\n```", true);
            embed.AddField("WS Latency", "```ini\n[ " + ctx.Client.Ping + "ms ]\n```", true);

            await ctx.RespondAsync(embed);
        }
    }
}