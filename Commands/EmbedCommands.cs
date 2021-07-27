using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using DSharpPlus;

namespace DiscordBot.Commands
{
    [Group("embed")]
    public class EmbedCommands : BaseCommandModule
    {
        [Command("create")]
        [Description("Creates an embed")]
        public async Task CreateEmbed(CommandContext context, [Description("The Json string to convert into an Embed")] string embedJson)
        {
            await context.Message.DeleteAsync();
            embedJson = embedJson.Replace("```", "");
            var embed = new SimpleEmbed();

            embed = JsonConvert.DeserializeObject<SimpleEmbed>(embedJson);

            var embedBuilder = embed.ToEmbed();

            await context.Channel.SendMessageAsync(embedBuilder);
        }

        [Command("replace")]
        [Description("Replaces an embed")]
        public async Task ReplaceEmbed(CommandContext ctx, ulong id)
        {
            Console.WriteLine(Formatter.Mention(ctx.Guild.GetRole(801397281802682420)));
            await ctx.Message.DeleteAsync();

            var interactivity = ctx.Client.GetInteractivity();

            var enterMessage = await ctx.Channel.SendMessageAsync("Enter the SimpleEmbed Json");

            var reply = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
            var embedJson = reply.Result.Content;

            embedJson = embedJson.Replace("```", "");

            await enterMessage.DeleteAsync();
            await reply.Result.DeleteAsync();

            try
            {
                var embed = new SimpleEmbed();
                var embedBuilder = new DiscordEmbedBuilder();

                embed = JsonConvert.DeserializeObject<SimpleEmbed>(embedJson);

                embedBuilder = embed.ToEmbed();

                var message = await ctx.Channel.GetMessageAsync((ulong)id);
                await message.ModifyAsync(new DiscordMessageBuilder().WithEmbed(embedBuilder));
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder()
                    .WithTitle("Error")
                    .WithColor(DiscordColor.Red)
                    .WithDescription(e.ToString()));
                return;
            }
            await ctx.Channel.SendMessageAsync("Successfully replaced embed");
        }

        [Command("get")]
        [Description("Gets an embed")]
        public async Task GetEmbed(CommandContext ctx, ulong id)
        {
            await ctx.Channel.SendMessageAsync("```" + id + "```");
            var message = await ctx.Channel.GetMessageAsync((ulong) id);
            var embed = message.Embeds.FirstOrDefault();

            var simpleEmbed = new SimpleEmbed();
            
            simpleEmbed = new SimpleEmbed(embed);

            await ctx.Channel.SendMessageAsync("```" + JsonConvert.SerializeObject(simpleEmbed) + "```");
        }

        public class SimpleEmbed
        {
            public string Title;
            public string Url;
            public string Description;
            public string Color;
            public string Image;
            public EmbedAuthor Author = new();
            public EmbedFooter Footer = new();
            public EmbedThumbnail Thumbnail = new();
            public List<EmbedField> Fields = new();

            public SimpleEmbed() { }

            public SimpleEmbed(DiscordEmbed embed)
            {
                Title = embed.Title;

                if (embed.Url != null)
                    Url = embed.Url.ToString();

                Description = embed.Description;

                if (embed.Color != null)
                    Color = embed.Color.Value.Value.ToString("X6");

                if (embed.Image != null)
                    Image = embed.Image.Url.ToString();

                if (embed.Author != null)
                {
                    Author.Name = embed.Author.Name;

                    if (embed.Author.Url != null)
                        Author.Url = embed.Author.Url.ToString();

                    if (embed.Author.IconUrl != null)
                        Author.IconUri = embed.Author.IconUrl.ToString();
                }
                
                if (embed.Footer != null)
                {
                    Footer.Text = embed.Footer.Text;

                    if (embed.Footer.IconUrl != null)
                        Footer.IconUri = embed.Footer.IconUrl.ToString();
                }
                
                if (embed.Thumbnail != null)
                {
                    if (embed.Thumbnail.Url != null)
                        Thumbnail.Uri = embed.Thumbnail.Url.ToString();

                    Thumbnail.Height = embed.Thumbnail.Height;
                    Thumbnail.Width = embed.Thumbnail.Width;
                }

                foreach (var field in embed.Fields)
                {
                    Fields.Add(new EmbedField
                    {
                        Name = field.Name,
                        Value = field.Value,
                        Inline = field.Inline
                    });
                }
            }

            public DiscordEmbedBuilder ToEmbed()
            {
                var embed =  new DiscordEmbedBuilder()
                .WithTitle(Title)
                .WithUrl(Url)
                .WithDescription(Description)
                .WithColor(new DiscordColor(Color))
                .WithImageUrl(Image)
                .WithThumbnail(Thumbnail.Uri, Thumbnail.Height, Thumbnail.Width)
                .WithAuthor(Author.Name, Author.Url, Author.IconUri)
                .WithFooter(Footer.Text, Footer.IconUri);


                foreach (var field in Fields)
                {
                    embed.AddField(field.Name, field.Value, field.Inline);
                }

                return embed;
            }

            public class EmbedAuthor
            {
                public string Name;
                public string Url;
                public string IconUri;
            }

            public class EmbedFooter
            {
                public string Text;
                public string IconUri;
            }

            public class EmbedThumbnail
            {
                public string Uri;
                public int Width;
                public int Height; 
            }

            public class EmbedField
            {
                public string Name;
                public string Value;
                public bool Inline;
            }
        }
    }
}