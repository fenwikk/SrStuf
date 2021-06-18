using System;
using System.Linq;
using System.Collections.Generic;
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

            var prefixes = new List<string>();
            string longPrefix = Bot.Config.GetPrefix(ctx.Guild.Id);
            prefixes.Add(longPrefix);
            
            if (char.IsLetter(longPrefix.ToCharArray().First()) && longPrefix.Length > 2)
            {
                var shortPrefix = longPrefix.ToCharArray().First() + "!";
                prefixes.Add(shortPrefix);
            }
            
            embed.AddField("Prefix(es)", "`" + string.Join("`, `", prefixes) + "`");

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