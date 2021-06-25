using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DiscordBot;

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;

using OxfordDictionariesAPI;
using OxfordDictionariesAPI.Models;

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

        [SlashCommand("HaveIBeenPwned", "Have You Been Pwned?")]
        public async Task Pwned(InteractionContext ctx, [Option("Password", "Password to check")]string password)
        {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.DeferredChannelMessageWithSource);
            var pwned = new HaveIBeenPwned.Password.HaveIBeenPwned();
            var timesPswrdPwned = pwned.GetNumberOfTimesPasswordPwned(password);

            var embed = new DiscordEmbedBuilder().WithTitle("Have I Been Pwned?");
            if (timesPswrdPwned == 0)
            {
                embed.WithDescription("No, your password wasn't found on HaveIBeenPwned.com")
                    .WithColor(DiscordColor.SapGreen);
            }
            else
            {
                embed.WithDescription($"Yes, your password has been pwned **{timesPswrdPwned}** times!")
                    .WithColor(DiscordColor.IndianRed);
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
        
        [SlashCommand("dictionary", "Returns the definition of the word if valid")]
        public async Task Dictionary(InteractionContext ctx, [Option("Word", "American english word to define")]string word)
        {
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.DeferredChannelMessageWithSource);
            var client = new OxfordDictionaryClient("2583c247", "1c882daaf2c97abe1d9779e1af8724ab");
            
            var searchResult = client.SearchEntries(word, CancellationToken.None).Result;
            // result is present
            Assert.True(searchResult != null);
            // result is not empty
            Assert.True(searchResult.Results.Length > 0);
            // result match
            Result result = searchResult.Results.First();
            Assert.True(result.Id == word && result.Word == word);

            var embed = new DiscordEmbedBuilder().WithTitle("Definition");
            foreach (var entry in result.LexicalEntries)
            {
                var senses = new List<string>();
                foreach (var entryEntry in entry.Entries)
                {
                    foreach (var sense in entryEntry.Senses)
                    {
                        senses.Add($"{sense.Definitions.FirstOrDefault()}\n*\"{sense.Examples.FirstOrDefault()}\"*");
                    }
                }
                embed.AddField(entry.LexicalCategory, string.Join("\n\n", senses));
            }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}