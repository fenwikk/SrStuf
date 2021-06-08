using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace DiscordBot.Bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void WriteTitle()
        {
            Console.WriteLine(@"   _____ _ _     _        _____       _    _____     _     ");
            Console.WriteLine(@"  |   __|_| |_ _|_|___   |     |___ _| |  | __  |___| |_   ");
            Console.WriteLine(@"  |__   | | | | | | .'|  | | | | . | . |  | __ -| . |  _|  ");
            Console.WriteLine(@"  |_____|_|_|\_/|_|__,|  |_|_|_|___|___|  |_____|___|_|    ");
            Console.WriteLine("");
            Console.WriteLine(@"+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(@"---------------- 2021 (c) Filip Mårtensson ----------------");
            Console.WriteLine(@"+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
        }
    }
}
