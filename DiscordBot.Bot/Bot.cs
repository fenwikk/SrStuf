using System;
using System.IO;
using System.Threading.Tasks;

using DiscordBot.Bot.Commands;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBot.Bot
{
    public class Bot
    {
        public static DiscordClient Client { get; set; }
        public static InteractivityExtension Interactivity { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        public static PrefixResolverDelegate PrefixResolver { get; set; }
        public static Configuration Config { get; set; }

        public static string configFile = "config.json";
        
        public Bot(IServiceProvider services)
        {
            ConfigBot(services);

            Console.Clear();
            Program.WriteTitle();

            var activity = new DiscordActivity("you sleep", ActivityType.Watching);
            Client.ConnectAsync(activity);
        }
        
        public void ConfigBot(IServiceProvider services)
        {
            Console.Clear();
            Program.WriteTitle();

            Console.WriteLine("Configuring Bot...");
            Console.WriteLine("[1/4] Getting configuration");

            GetConfig();

            Console.WriteLine("[2/4] Creating and setting up client");

            CreateSetupClient(services);

            Console.WriteLine("[3/4] Registering commands");

            RegisterCommands();

            Console.WriteLine("Bot Loaded!");
        }

        public void GetConfig()
        {
            Config = new Configuration();
            
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Configuration file not found! Creating file...");
                Console.WriteLine("Please input your configuration:");
                Config.Setup(configFile);
            }

            Console.WriteLine("Deserializing configuration...");
            Config.Deserialize(configFile);
            
            while (Config.Token == null || Config.DefaultPrefix == null || Config.Token == string.Empty || Config.DefaultPrefix == string.Empty)
            {
                Console.WriteLine("Configuration not complete! Please fill in required fields:");
                Config.Setup(configFile);
            }
        }

        public void CreateSetupClient(IServiceProvider services)
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
                EnableDms = false,
                Services = services
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
            Commands.RegisterCommands<FunCommands>();
        }
        public static Task OnClientReady(DiscordClient client, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        public static Task<int> ResolvePrefixAsync(DiscordMessage msg)
        {
            string prefix = Config.GetPrefix(msg.Channel.GuildId);

            return Task.FromResult(msg.GetStringPrefixLength(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}