using Microsoft.EntityFrameworkCore;
using DiscordBot.DAL.Models;

namespace DiscordBot.DAL
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Guilds> Guilds { get; set; } 
    }
}