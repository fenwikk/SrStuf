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

        [Command("tictactoe")]
        [Aliases("ttt", "tictac")]
        public async Task TicTacToe(CommandContext ctx, DiscordUser opponent)
        {
            char[,] playfield =
            {
                {'0', '0', '0'},
                {'0', '0', '0'},
                {'0', '0', '0'}
            };
            var xTurn = false;

            var field = await ctx.RespondAsync(CreatePlayfield());

            ctx.Client.ComponentInteractionCreated += async (s, e) =>
            {
                if (e.User == (xTurn ? opponent : ctx.User))
                {
                    switch (e.Id)
                    {
                        case "1": playfield[0, 0] = xTurn ? 'X' : 'O'; break;
                        case "2": playfield[0, 1] = xTurn ? 'X' : 'O'; break;
                        case "3": playfield[0, 2] = xTurn ? 'X' : 'O'; break;

                        case "4": playfield[1, 0] = xTurn ? 'X' : 'O'; break;
                        case "5": playfield[1, 1] = xTurn ? 'X' : 'O'; break;
                        case "6": playfield[1, 2] = xTurn ? 'X' : 'O'; break;
                        
                        case "7": playfield[2, 0] = xTurn ? 'X' : 'O'; break;
                        case "8": playfield[2, 1] = xTurn ? 'X' : 'O'; break;
                        case "9": playfield[2, 2] = xTurn ? 'X' : 'O'; break;
                    }
                    xTurn = !xTurn;
                    await field.ModifyAsync(CreatePlayfield());
                }
            };

            DiscordMessageBuilder CreatePlayfield()
            {
                DiscordButtonComponent CreateTile(char tilePos, string buttonId) =>
                    new DiscordButtonComponent((tilePos == '0') ? ButtonStyle.Secondary : ((tilePos == 'X') ? ButtonStyle.Primary : ButtonStyle.Success), buttonId, "", (tilePos == '0') ? false : true, (tilePos == '0') ? null : new DiscordComponentEmoji((tilePos == 'X') ? "x" : "o"));

                return new DiscordMessageBuilder()
                    .AddComponents(new DiscordButtonComponent[]
                    {
                        CreateTile(playfield[0, 0], "1"),
                        CreateTile(playfield[0, 1], "2"),
                        CreateTile(playfield[0, 2], "3")
                    })
                    .AddComponents(new DiscordButtonComponent[]
                    {
                        CreateTile(playfield[1, 0], "4"),
                        CreateTile(playfield[1, 1], "5"),
                        CreateTile(playfield[1, 2], "6"),
                    })
                    .AddComponents(new DiscordButtonComponent[]
                    {
                        CreateTile(playfield[2, 0], "7"),
                        CreateTile(playfield[2, 1], "8"),
                        CreateTile(playfield[2, 2], "9"),
                    });
            }
        }

        [Command("tiky")]
        public async Task Tiky(CommandContext ctx)
        {
            await ctx.RespondAsync(":cookie:");
        } 

        [Command("fbi")]
        public async Task FBI(CommandContext ctx)
        {
            await ctx.RespondAsync("​To whatever FBI agent is in this discord, I do not affiliate with these people and myself, and have absolutely no relation with this server whatsoever. I do not condone anything that is posted here, by people or by me.\n\nIn case of an investigation by any federal entity or similar, I do not have any involvement with this group or with the people in it, I do not know how got here, probably added by a third party, I do not support any actions by members of this group");
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