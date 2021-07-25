using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using OxfordDictionariesAPI;
using OxfordDictionariesAPI.Models;
using Newtonsoft.Json;

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

            var embed = Bot.CreateEmbed(ctx)
                .WithFooter("Made by " + author.Username + "#" + author.Discriminator, author.AvatarUrl)
                .WithThumbnail(ctx.Guild.CurrentMember.AvatarUrl);

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

            embed.AddField("Support", "If you have questions, suggestions, or found a bug, please report it on Github by clicking the support button below");

            await ctx.RespondAsync(new DiscordMessageBuilder()
                .WithEmbed(embed)
                .AddComponents(new DiscordLinkButtonComponent[]
                {
                    new DiscordLinkButtonComponent("http://bit.ly/SeñorStuf", "Invite Me!"),
                    new DiscordLinkButtonComponent("http://bit.ly/StufSupport", "Support")
                }));
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

        [Command("haveibeenpwned")]
        [Description("Have you been Pwned?")]
        [Aliases("pwned")]
        public async Task Pwned(CommandContext ctx, string password)
        {
            var pwned = new HaveIBeenPwned.Password.HaveIBeenPwned();
            var timesPswrdPwned = pwned.GetNumberOfTimesPasswordPwned(password);

            var embed = Bot.CreateEmbed(ctx).WithTitle("Have I Been Pwned?");
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

            await ctx.RespondAsync(embed);
        }
        
        [Command("dictionary")]
        [Description("Returns the definition of the word if valid")]
        public async Task Dictionary(CommandContext ctx, [Description("American english word to define")]string word)
        {
            var client = new OxfordDictionaryClient("2583c247", "1c882daaf2c97abe1d9779e1af8724ab");
            
            var searchResult = client.SearchEntries(word, CancellationToken.None).Result;
            // result is present
            Assert.True(searchResult != null);
            // result is not empty
            Assert.True(searchResult.Results.Length > 0);
            // result match
            Result result = searchResult.Results.First();
            Assert.True(result.Id == word && result.Word == word);

            var definitionEmbed = Bot.CreateEmbed(ctx).WithTitle("Definition");
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
                definitionEmbed.AddField(entry.LexicalCategory, string.Join("\n\n", senses));
            }
            await ctx.RespondAsync(definitionEmbed);
        }

        [Command("embed")]
        [Description("Creates an embed")]
        public async Task DiscordEmbed(CommandContext context, string embedJson)
        {
            await context.Message.DeleteAsync();
            var embed = JsonConvert.DeserializeObject<DiscordEmbed>(embedJson);
            await context.Channel.SendMessageAsync(embed);
        }
    }
}