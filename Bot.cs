using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiscordBot.Commands;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBot
{
    public class Bot
    {
        public static DiscordClient Client { get; set; }
        public static InteractivityExtension Interactivity { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        public static PrefixResolverDelegate PrefixResolver { get; set; }
        public static Configuration Config { get; set; }

        public const string configFile = "config.json";
        
        public async Task RunAsync()
        {
            Program.WriteTitle();

            var activity = new DiscordActivity("you sleep", ActivityType.Watching);
            await Client.ConnectAsync(activity);
            
            await Task.Delay(-1);
        }

        public void GetConfig()
        {
            Config = new Configuration();
            
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Configuration file not found! Creating file...");
                Console.WriteLine("Please input your configuration. Fields marked with a asterisk (*) are mandatory");
                Config.Setup(configFile);
            }

            Console.WriteLine("Deserializing configuration...");
            Config.Deserialize(configFile);

            Console.WriteLine(Config.Token);
            Console.WriteLine(Config.DefaultPrefix);
            
            while (Config.Token == null || Config.DefaultPrefix == null || Config.Token == string.Empty || Config.DefaultPrefix == string.Empty)
            {
                Console.WriteLine("Configuration not complete! Please fill in required fields:");
                Config.Setup(configFile);
            }
        }

        public void CreateSetupClient()
        {
            var config = new DiscordConfiguration
            {
                Token = Config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                Intents = DiscordIntents.AllUnprivileged                
            };

            var commandsConfig = new CommandsNextConfiguration
            {
                PrefixResolver = ResolvePrefixAsync,
                EnableMentionPrefix = true,
                EnableDms = false
            };

            var interactivityConfig = new InteractivityConfiguration()
            {
                PaginationBehaviour = default,
            };

            Client = new DiscordClient(config);
            Client.Ready += OnClientReady;
            
            Commands = Client.UseCommandsNext(commandsConfig);
            Client.UseInteractivity(interactivityConfig);
        }

        public void RegisterCommands()
        {
            Commands.RegisterCommands<UtilityCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<ModulesCommands>();
        }
        public static Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        public static Task<int> ResolvePrefixAsync(DiscordMessage msg)
        {
            List<string> prefixes = new List<string>();

            string longPrefix = Config.GetPrefix(msg.Channel.GuildId);
            prefixes.Add(longPrefix);
            
            if (char.IsLetter(longPrefix.ToCharArray().First()) && longPrefix.Length > 2)
            {
                var shortPrefix = longPrefix.ToCharArray().First() + "!";
                prefixes.Add(shortPrefix);
            }

            foreach (var prefix in prefixes)
            {
                if (msg.GetStringPrefixLength(prefix, StringComparison.OrdinalIgnoreCase) != -1)
                    return Task.FromResult(msg.GetStringPrefixLength(prefix, StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult(-1);
        }

        public static DiscordEmbedBuilder CreateEmbed(CommandContext ctx)
        {
            string time = ctx.Message.Timestamp.ToString("HH:mm");

            return new DiscordEmbedBuilder
            {
                Color = ctx.Guild.CurrentMember.Color,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {                
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = ctx.Member.DisplayName + " • Today at " + time
                }      
            };
        }
    }
}