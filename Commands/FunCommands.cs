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
                    json = w.DownloadString("https://meme-api.herokuapp.com/gimme");
                }
                catch (Exception) {}
                var meme = JsonConvert.DeserializeObject<Meme>(json);
                var embed = Bot.CreateEmbed(ctx)
                    .WithTitle(meme.title)
                    .WithUrl(meme.postLink)
                    .WithImageUrl(meme.url)
                    .WithFooter("👍 " + meme.ups + "   |   🏫  /r/" + meme.subreddit);

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
                    json = w.DownloadString("https://meme-api.herokuapp.com/gimme/" + subreddit);
                }
                catch (Exception) {}
                var meme = JsonConvert.DeserializeObject<Meme>(json);
                var embed = Bot.CreateEmbed(ctx)
                    .WithTitle(meme.title)
                    .WithUrl(meme.postLink)
                    .WithImageUrl(meme.url)
                    .WithFooter("👍 " + meme.ups + "   |   🏫  /r/" + meme.subreddit);

                await ctx.RespondAsync(embed);
            }
        } 

        [Command("meme")]
        public async Task Gimme(CommandContext ctx, int amount)
        {
            List<Meme> memes = new List<Meme>();

            for (int i = 0; i < amount; i++)
            {    
                using (var w = new WebClient()) 
                {
                    var json = string.Empty;
                    try 
                    {
                        json = w.DownloadString("https://meme-api.herokuapp.com/gimme");
                    }
                    catch (Exception) {}
                    var meme = JsonConvert.DeserializeObject<Meme>(json);
                    
                    var embed = Bot.CreateEmbed(ctx)
                        .WithTitle(meme.title)
                        .WithUrl(meme.postLink)
                        .WithImageUrl(meme.url)
                        .WithFooter("👍 " + meme.ups + "   |   🏫  /r/" + meme.subreddit);

                    await ctx.RespondAsync(embed);
                }
            }
        } 

        [Command("meme")]
        public async Task Gimme(CommandContext ctx, string subreddit, int amount)
        {
            for (int i = 0; i < amount; i++)
            {    
                using (var w = new WebClient()) 
                {
                    var json = string.Empty;
                    try 
                    {
                        json = w.DownloadString("https://meme-api.herokuapp.com/gimme/" + subreddit);
                    }
                    catch (Exception) {}
                    var meme = JsonConvert.DeserializeObject<Meme>(json);
                    var embed = Bot.CreateEmbed(ctx)
                        .WithTitle(meme.title)
                        .WithUrl(meme.postLink)
                        .WithImageUrl(meme.url)
                        .WithFooter("👍 " + meme.ups + "   |   🏫  /r/" + meme.subreddit);

                    await ctx.RespondAsync(embed);
                }
            }
        } 

        class Meme
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