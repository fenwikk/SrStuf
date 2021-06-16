using System;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Commands
{
    public class UtilityCommands : BaseCommandModule
    {
        [Command("info")]
        [Description("Returns info on the bot")]
        [Aliases("i")]
        public async Task Info(CommandContext ctx)
        {
            var author = await ctx.Client.GetUserAsync(Bot.Config.AuthorId);

            var embed = Bot.CreateEmbed(ctx);
            embed.WithFooter("Made by " + author.Username + "#" + author.Discriminator, author.AvatarUrl);
            
            embed.Title = ctx.Client.CurrentUser.Username + " Info";
            embed.Description = Bot.Config.Description;

            await ctx.RespondAsync(embed);
        }

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