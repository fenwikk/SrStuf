using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DiscordBot
{
    [Serializable]
    public class Configuration
    {
        public string Token { get; set; }
        public ulong AuthorId { get; set; }
        public string DefaultPrefix { get; set; }
        public List<Guild> Guilds { get; set; }
        public string Description { get; set; }
        public TimeSpan Timeout { get; set; }

        public void Setup(string configFileName)
        {
            while (Token == string.Empty || Token == null)
            {
                Console.WriteLine("Token*: ");
                Token = Console.ReadLine();
            }
            while (DefaultPrefix == string.Empty || DefaultPrefix == null)
            {
                Console.WriteLine("\nDefault Prefix*: ");
                DefaultPrefix= Console.ReadLine();
            }
            while (AuthorId == 0)
            {
                Console.WriteLine("\nAuthor Id (default: 393368613652004877): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out var id))
                {
                    if (id.ToString().Length == 18)
                        AuthorId = (ulong)id;
                    else
                        Console.WriteLine("Input must be a Discord Snowflake (integer with 18 digits)");
                }
                else
                {
                    if (input == string.Empty)
                        AuthorId = 393368613652004877;
                    else
                        Console.WriteLine("Input must be a Discord Snowflake (integer with 18 digits)");
                }
            }
            while (Timeout == TimeSpan.Zero)
            {
                Console.WriteLine("\nInteraction Timeout (in seconds, defaults to 60): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out var seconds))
                    Timeout = TimeSpan.FromSeconds(seconds);
                else
                {
                    if (input == string.Empty)
                        Timeout = TimeSpan.FromSeconds(60);
                    else
                        Console.WriteLine("Input must be an integer");
                }
            }
            while (Description == string.Empty || Description == null)
            {
                Console.WriteLine("\nBot Description: ");
                Description = Console.ReadLine();
                if (Description == string.Empty || Description == null)
                    Description = "No description provided";
            }

            Guilds = new List<Guild>() { new Guild { id = 0, prefix = DefaultPrefix }};

            Serialize(configFileName);
        }
        public void Serialize(string configFileName = Bot.configFile)
        {
            string jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText(configFileName, jsonString);
        }

        public void Deserialize(string configFileName = Bot.configFile)
        {
            string jsonString = File.ReadAllText(configFileName);
            Configuration config;

            config = JsonConvert.DeserializeObject<Configuration>(jsonString);

            Token = config.Token;
            DefaultPrefix = config.DefaultPrefix;
            Guilds = config.Guilds;
            AuthorId = config.AuthorId;
            Description = config.Description;
        }

        public string GetPrefix(ulong? guildId)
        {
            foreach (var guild in Guilds)
            {
                if (guild.id == guildId)
                    return guild.prefix;
            }
            
            Guilds.Add(new Guild
            {
                id = (ulong)guildId,
                prefix = DefaultPrefix
            });
            
            Serialize(Bot.configFile);

            return DefaultPrefix;
        }
        
        public Guild GetGuild(ulong? guildId)
        {
            foreach (var guild in Guilds)
            {
                if (guild.id == guildId)
                    return guild;
            }
            
            var newGuild = new Guild
            {
                id = (ulong)guildId,
                prefix = DefaultPrefix
            };

            Guilds.Add(newGuild);
            
            Serialize(Bot.configFile);

            return newGuild;
        }
    }
    public class Guild
    {
        public ulong id;
        public string prefix;
        public bool welcomeModule = true;
        public ulong welcomeChannel;
        public string welcomeMessage = "Welcome {user}!";
    }
}