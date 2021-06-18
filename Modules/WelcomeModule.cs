using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class WelcomeModule
    {
        public DiscordChannel welcomeChannel;

        #region Teachers

        public static List<Teacher> teachers = new List<Teacher>()
        {
            new Teacher
            {
                Name = "Silvia",
                AvatarUri = "https://i.imgur.com/kskvnkC.jpg",
                Subject = "Spanish",

                Sentences = new List<string>()
                {
                    "{user}! That's 3 minutes absence! What's sorry I'm late in Spanish?",
                    "Bueno, mira quien decidió presentarse hoy, {user}!"
                }
            },
            new Teacher
            {
                Name = "Sara",
                // AvatarUri = "";
                Subject = "Math",

                Sentences = new List<string>()
                {
                    "{user}! How can you be late to the most important class of your life??",
                    "Maybe {user} can prove why this triangle is a triangle since they're **late**!",
                    "Welcome {user}! Now, prove that 1 = 2!"
                }
            },
            new Teacher
            {
                Name = "Mattias",
                // AvatarUri = "";
                Subject = "P.E.",

                Sentences = new List<string>()
                {
                    "{user}? Late? I've got class today?"
                }
            },
            new Teacher
            {
                Name = "Anna",
                // AvatarUri = "";
                Subject = "Biology",

                Sentences = new List<string>()
                {
                    "{user}! Ah, it's ok, we were just going to dissect this frog :grin:"
                }
            }
        };

        #endregion

        public static async Task Welcome(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            var guild = Bot.Config.GetGuild(e.Guild.Id);

            var embed = CreateEmbed(e.Member);
            DiscordChannel channel = await sender.GetChannelAsync(Bot.Config.GetGuild(e.Guild.Id).welcomeChannel);

            await channel.SendMessageAsync(embed).ConfigureAwait(false);
            await channel
                .SendMessageAsync(guild.welcomeMessage
                .Replace("{user}", e.Member.Mention));
        }

        public static DiscordEmbedBuilder CreateEmbed(DiscordUser user)
        {
            var random = new Random();

            var teacher = teachers[random.Next(teachers.Count)];
            var sentence = teacher.Sentences[random.Next(teacher.Sentences.Count)];

            sentence = sentence.Replace("{user}", user.Mention);

            var embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = teacher.Name,
                    IconUrl = teacher.AvatarUri
                },
                Title = "You're late for " + teacher.Subject + " class!",
                Description = sentence,
                Color = new DiscordColor("#dc4e60")
            };

            return embed;
        }
        public class Teacher
        {
            public string Name;
            public string AvatarUri;
            public string Subject;

            public List<string> Sentences;
        }
    }
}
