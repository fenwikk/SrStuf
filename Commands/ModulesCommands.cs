using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using DiscordBot.Modules;

using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot.Commands
{
    [Group("modules")]
    public class ModulesCommands : BaseCommandModule
    {
        [GroupCommand()]
        [Description("Displays all the modules")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Modules(CommandContext ctx)
        {
            var guild = Bot.Config.GetGuild(ctx.Guild.Id);
            var baseEmbed = Bot.CreateEmbed(ctx);
            
            var buttons = new DiscordButtonComponent[2];
            var messages = new List<DiscordMessage>();

            Console.WriteLine("setup");

            if (guild.welcomeModule == true)
            {
                buttons = new DiscordButtonComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Secondary, "editWelcome", "Edit"),
                    new DiscordButtonComponent(ButtonStyle.Danger, "welcomeModule", "Disable")
                };
            }
            else
            {
                buttons = new DiscordButtonComponent[]
                {
                    new DiscordButtonComponent(ButtonStyle.Secondary, "editWelcome", "Edit", true),
                    new DiscordButtonComponent(ButtonStyle.Success, "welcomeModule", "Enable")
                };
            }

            var welcome = await ctx.Channel
                .SendMessageAsync(new DiscordMessageBuilder()
                    .WithEmbed(baseEmbed
                        .WithTitle("Welcome Module")
                        .WithDescription("Welcomes new users to the server!"))
                    .AddComponents(buttons));
            messages.Add(welcome);

            
            var buttonPressed = string.Empty;
            ctx.Client.ComponentInteractionCreated += async (s, e) =>
            {
                buttonPressed = e.Id;
                foreach (var message in messages)
                {
                    await message.DeleteAsync();
                }
            };

            while (buttonPressed == string.Empty);

            if (buttonPressed == "welcomeModule")
            {
                if (guild.welcomeModule)
                {
                    guild.welcomeModule = false;
                    await ctx.RespondAsync("Welcome Module set to: Disabled");
                }
                else
                {
                    guild.welcomeModule = true;
                    await ctx.RespondAsync("Welcome Module set to: Enabled");
                }
            }
        }

        [Command("welcome")]
        [Description("Edits the Welcome Module")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Welcome(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var messages = new List<DiscordMessage>();

            var guild = Bot.Config.GetGuild(ctx.Guild.Id);

            var editingMessage = await ctx.RespondAsync("Editing the Welcome Module\nPreview:");
            
            if (guild.welcomeModule)
            {
                string welcomeChannel;
                if (guild.welcomeChannel != 0)
                {
                    var channel = await ctx.Client.GetChannelAsync(guild.welcomeChannel);

                    welcomeChannel = "In " + channel.Mention;
                }
                else
                {
                    welcomeChannel = "In *no Welcome Channel set*";
                }

                var welcomeEmbed = await ctx.Channel
                    .SendMessageAsync(new DiscordMessageBuilder()
                    .WithContent(welcomeChannel)
                    .WithEmbed(WelcomeModule.CreateEmbed(ctx.User)));
                    
                var welcomeMessage = await ctx.Channel
                    .SendMessageAsync(guild.welcomeMessage
                    .Replace("{user}", ctx.Member.Mention));

                var buttonBuilder = new DiscordMessageBuilder()
                    .WithContent("mknskjn")
                    .AddComponents(new DiscordButtonComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Primary, "channel", "Edit Channel"),
                        new DiscordButtonComponent(ButtonStyle.Primary, "message", "Edit Message"),
                        new DiscordButtonComponent(ButtonStyle.Danger, "disable", "Disable Module")
                    });

                await ctx.Channel.SendMessageAsync(buttonBuilder);

                var buttonPressed = string.Empty;
                ctx.Client.ComponentInteractionCreated += async (s, e) =>
                {
                    buttonPressed = e.Id;
                    await e.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder());
                };

                while (buttonPressed == string.Empty);

                if (buttonPressed == "channel") // Edit Welcome Channel
                {
                    bool validResult = false;
                    var message = await ctx.RespondAsync("Editing the Welcome Channel\nPlease mention the new Welcome Channel:");
                    messages.Add(message);


                    while (!validResult)
                    {
                        var newChannel = await interactivity.WaitForMessageAsync(x => x != null);
                        messages.Add(newChannel.Result);
                        Console.WriteLine(newChannel.Result.Content);

                        if (newChannel.Result.MentionedChannels != null)
                        {
                            await welcomeEmbed.ModifyAsync(new DiscordMessageBuilder()
                                .WithContent("In " + newChannel.Result.MentionedChannels.FirstOrDefault().Mention)
                                .WithEmbed(WelcomeModule.CreateEmbed(ctx.User)));

                            guild.welcomeChannel = newChannel.Result.MentionedChannels.FirstOrDefault().Id;
                            validResult = true;
                        }
                        else
                        {
                            var repeatMessage = await ctx.Channel.SendMessageAsync("Please mention a channel to continue");
                        }
                    }
                }
                else if (buttonPressed == "message") // Edit Welcome Message
                {
                    var message = await ctx.RespondAsync("Editing the Welcome Message\nPlease enter the new message: (replace th user mention with \"*{user}*\")");
                    messages.Add(message);

                    var newMessage = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
                    messages.Add(newMessage.Result);

                    await welcomeMessage.ModifyAsync(newMessage.Result.Content);
                    
                    guild.welcomeMessage = newMessage.Result.Content;
                }
                else if (buttonPressed == "disable") // Turn off the Welcome Module
                {
                    guild.welcomeModule = false;

                    await welcomeEmbed.DeleteAsync();
                    await welcomeMessage.DeleteAsync();
                }

            }
            else
            {
                var message = await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                    .WithContent("Welcome Module turned off")
                    .AddComponents(new DiscordButtonComponent[]
                    {
                        new DiscordButtonComponent(ButtonStyle.Success, "enable", "Enable Module")
                    }));

                bool buttonPressed = false;
                ctx.Client.ComponentInteractionCreated += async (s, e) =>
                {
                    if (e.User == ctx.User)
                    {
                        await e.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder());
                        guild.welcomeModule = true;

                        string welcomeChannel;
                        if (guild.welcomeChannel != 0)
                        {
                            var channel = await ctx.Client.GetChannelAsync(guild.welcomeChannel);

                            welcomeChannel = "In " + channel.Mention;
                        }
                        else
                        {
                            welcomeChannel = "In *no Welcome Channel set*";
                        }

                        var welcomeEmbed = await ctx.Channel
                            .SendMessageAsync(new DiscordMessageBuilder()
                            .WithContent(welcomeChannel)
                            .WithEmbed(WelcomeModule.CreateEmbed(ctx.User)));
                            
                        var welcomeMessage = await ctx.Channel
                            .SendMessageAsync(guild.welcomeMessage
                            .Replace("{user}", ctx.Member.Mention));
                        
                        buttonPressed = true;
                    }
                    else
                        await e.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.Pong);
                };

                while (!buttonPressed);
            }

            await DeleteAllMessagesAsync(messages);
            Bot.Config.Serialize();
            await editingMessage.ModifyAsync("New Welcome Message:");

            if (!guild.welcomeModule)
                await ctx.Channel.SendMessageAsync("Welcome Module turned off");
        }

        [Command("prefix")]
        [Description("Edits the prefix")]
        [RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public async Task Prefix(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var messages = new List<DiscordMessage>();

            var guild = Bot.Config.GetGuild(ctx.Guild.Id);

            var editingMessage = await ctx.RespondAsync("Editing the Prefix\nCurrent: `" + guild.prefix + "`\nPlease enter the new prefix:");
            
            var newPrefix = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
            messages.Add(newPrefix.Result);

            guild.prefix = newPrefix.Result.Content;
            Bot.Config.Serialize();

            await editingMessage.ModifyAsync("New prefix: `" + guild.prefix + "`");
            await DeleteAllMessagesAsync(messages);
        }

        [Command("prefix")]
        public async Task Prefix(CommandContext ctx, string prefix)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var messages = new List<DiscordMessage>();

            var guild = Bot.Config.GetGuild(ctx.Guild.Id);

            guild.prefix = prefix;
            Bot.Config.Serialize();

            await ctx.RespondAsync("New prefix set to: `" + guild.prefix + "`");
            await DeleteAllMessagesAsync(messages);
        }



        public async Task DeleteAllMessagesAsync(List<DiscordMessage> messages)
        {
            foreach (var message in messages)
            {
                await message.DeleteAsync();
            }
        }
    }
}