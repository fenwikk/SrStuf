
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiscordBot;

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Commands.SlashCommands
{
    public class UtilityCommands : SlashCommandModule
    {
        [SlashCommand("info", "Returns info on the bot")]
        public async Task Info(InteractionContext ctx)
        {
            var author = await Bot.Client.GetUserAsync(Bot.Config.AuthorId);

            var embed = new DiscordEmbedBuilder()
                .WithFooter("Made by " + author.Username + "#" + author.Discriminator, author.AvatarUrl)
                .WithThumbnail(ctx.Interaction.Guild.CurrentMember.AvatarUrl);

            embed.Title = Bot.Client.CurrentUser.Username + " Info";
            embed.Description = Bot.Config.Description;

            var prefixes = new List<string>();
            string longPrefix = Bot.Config.GetPrefix(ctx.Interaction.Guild.Id);
            prefixes.Add(longPrefix);
            
            if (char.IsLetter(longPrefix.ToCharArray().First()) && longPrefix.Length > 2)
            {
                var shortPrefix = longPrefix.ToCharArray().First() + "!";
                prefixes.Add(shortPrefix);
            }
            
            embed.AddField("Prefix(es)", "`" + string.Join("`, `", prefixes) + "`");

            embed.AddField("Support", "If you have questions, suggestions, or found a bug, please report it on Github by clicking the support button below");

            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(new DiscordLinkButtonComponent[]
                {
                    new DiscordLinkButtonComponent("http://bit.ly/SeñorStuf", "Invite Me!"),
                    new DiscordLinkButtonComponent("http://bit.ly/StufSupport", "Support")
                })));
        }

        [Command("ping")]
        [Description("Returns \"Pong!\"")]
        public async Task Ping(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pong!"));

            var latency = (DateTime.UtcNow - ctx.Interaction.CreationTimestamp).ToString("fff");

            var embed = new DiscordEmbedBuilder()
                .WithTitle("Pong!")
                .AddField("Latency", "```ini\n[ " + latency + "ms ]\n```", true)
                .AddField("WS Latency", "```ini\n[ " + ctx.Client.Ping + "ms ]\n```", true);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("").AddEmbed(embed));
        }
    }
}