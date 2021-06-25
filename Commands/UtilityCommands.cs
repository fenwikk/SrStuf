using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Globalization;

using HaveIBeenPwned.Password;

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
            using (var w = new WebClient()) 
            {
                var json = string.Empty;
                try 
                {
                    json = w.DownloadString("https://api.dictionaryapi.dev/api/v2/entries/en_US/" + word);
                    Console.WriteLine(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                var wordDefinition = JsonConvert.DeserializeObject<Definition>(json);
                Console.WriteLine(wordDefinition);

                var phonetics = new List<string>();
                foreach (var phonetic in wordDefinition.Phonetics)
                {
                    phonetics.Add(phonetic.Text);
                }
                var wordEmbed = Bot.CreateEmbed(ctx)
                    .WithTitle(wordDefinition.Word)
                    .WithUrl("https://www.lexico.com/en/definition/" + wordDefinition.Word)
                    .AddField("Phonetics", "`" + string.Join("`, `", phonetics) + "`");

                await ctx.RespondAsync(wordEmbed);
                

                var meaningEmbeds = new List<DiscordEmbedBuilder>();

                TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;
                foreach (var meaning in wordDefinition.Meanings)
                {
                    var newMeaning = new DiscordEmbedBuilder()
                        .WithFooter("", "")
                        .WithTitle(textInfo.ToTitleCase(meaning.PartOfSpeech));
                    
                    foreach (var definition in meaning.Definitions)
                    {
                        newMeaning.AddField(definition.DefinitionDefinition, "*" + definition.Example + "*");
                        newMeaning.AddField("Synonyms", "`" + string.Join("`, `", definition.Synonyms) + "`");
                    }
                    
                    await ctx.Channel.SendMessageAsync(newMeaning);
                }
            }
        }

        public partial class Definition
        {
            [JsonProperty("word")]
            public string Word { get; set; }

            [JsonProperty("phonetics")]
            public Phonetic[] Phonetics { get; set; }

            [JsonProperty("meanings")]
            public Meaning[] Meanings { get; set; }
        }

        public partial class Meaning
        {
            [JsonProperty("partOfSpeech")]
            public string PartOfSpeech { get; set; }

            [JsonProperty("definitions")]
            public Definition[] Definitions { get; set; }
        }

        public partial class Definition
        {
            [JsonProperty("definition")]
            public string DefinitionDefinition { get; set; }

            [JsonProperty("synonyms")]
            public string[] Synonyms { get; set; }

            [JsonProperty("example")]
            public string Example { get; set; }
        }

        public partial class Phonetic
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("audio")]
            public Uri Audio { get; set; }
        }
    }
}