using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("meme")]
        [Description("Gets a meme from a land far, far away, in a great kingdom known as *Reddit*")]
        [Aliases("gimme")]
        public async Task Gimme(CommandContext ctx)
        {
            using (var w = new WebClient()) 
            {
                var json = string.Empty;
                try 
                {
                    json = w.DownloadString("https://meme-api.herokuapp.com/gimme/1");
                    
                        Console.WriteLine("1");
                }
                catch (Exception) {}
                var meme = JsonConvert.DeserializeObject<Memes>(json);
                var embed = Bot.CreateEmbed(ctx)
                    .WithTitle(meme.memes[0].title)
                    .WithUrl(meme.memes[0].postLink)
                    .WithImageUrl(meme.memes[0].url)
                    .WithFooter("👍 " + meme.memes[0].ups + "   |   🏫  /r/" + meme.memes[0].subreddit);

                await ctx.RespondAsync(embed);
            }
        } 

        [Command("meme")]
        public async Task Gimme(CommandContext ctx, string subreddit)
        {
            using (var w = new WebClient()) 
            {
                var json = string.Empty;
                try 
                {
                    json = w.DownloadString("https://meme-api.herokuapp.com/gimme/1" + subreddit);
                    
                        Console.WriteLine("json");
                }
                catch (Exception) {}
                var meme = JsonConvert.DeserializeObject<Memes>(json);
                var embed = Bot.CreateEmbed(ctx)
                    .WithTitle(meme.memes[0].title)
                    .WithUrl(meme.memes[0].postLink)
                    .WithImageUrl(meme.memes[0].url)
                    .WithFooter("👍 " + meme.memes[0].ups + "   |   🏫  /r/" + meme.memes[0].subreddit);

                await ctx.RespondAsync(embed);
            }
        } 

        [Command("tiky")]
        public async Task Tiky(CommandContext ctx)
        {
            await ctx.RespondAsync(":cookie:");
        } 

        class Memes
        {
            public List<Meme> memes;

            public class Meme
            {
                public string postLink;
                public string subreddit;
                public string title;
                public string url;
                public string nsfw;
                public string spoiler;
                public string author;
                public string ups;
            }
        }
    }
}