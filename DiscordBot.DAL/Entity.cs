using System.ComponentModel.DataAnnotations;

namespace DiscordBot.DAL.Models
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}