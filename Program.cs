using System;

namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();

            WriteTitle();

            Console.WriteLine("Loading Bot...");
            Console.WriteLine("[1/4] Getting configuration");

            bot.GetConfig();

            Console.WriteLine("[2/4] Creating and setting up client");

            bot.CreateSetupClient();

            Console.WriteLine("[3/4] Registering commands");

            bot.RegisterCommands();

            Console.WriteLine("Bot Loaded!");
            
            bot.RunAsync().GetAwaiter().GetResult();
        }

        public static void WriteTitle()
        {
            Console.Clear();                                
            Console.WriteLine(@"       ____  _                   _     _____     _         ");
            Console.WriteLine(@"      |    \|_|___ ___ ___ ___ _| |   | __  |___| |_       ");
            Console.WriteLine(@"      |  |  | |_ -|  _| . |  _| . |   | __ -| . |  _|      ");
            Console.WriteLine(@"      |____/|_|___|___|___|_| |___|   |_____|___|_|        ");         
            Console.WriteLine("");
            Console.WriteLine(@"+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(@"---------------- 2021 (c) Filip Mårtensson ----------------");
            Console.WriteLine(@"+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
        }
    }
}
