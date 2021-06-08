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
        public List<Guild> Guilds { get; set; }
        public string DefaultPrefix { get; set; }

        public void Setup(string configFileName)
        {
            if (Token == string.Empty || Token == null)
            {
                Console.WriteLine("Token: ");
                string token = Console.ReadLine(); 
                Token = token;
            }
            if (DefaultPrefix == string.Empty || DefaultPrefix == null)
            {
                Console.WriteLine("\nDefault Prefix: ");
                string defaultPrefix = Console.ReadLine();
                DefaultPrefix = defaultPrefix;
            }

            Guilds = new List<Guild>() { new Guild { id = 0, prefix = DefaultPrefix }};

            Serialize(configFileName);
        }
        public void Serialize(string configFileName)
        {
            string jsonString = JsonConvert.SerializeObject(this);
            File.WriteAllText(configFileName, jsonString);
        }

        public void Deserialize(string configFileName)
        {
            string jsonString = File.ReadAllText(configFileName);
            Configuration config;

            config = JsonConvert.DeserializeObject<Configuration>(jsonString);

            Token = config.Token;
            DefaultPrefix = config.DefaultPrefix;
            Guilds = config.Guilds;
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
    }
}