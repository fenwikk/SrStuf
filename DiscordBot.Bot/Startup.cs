using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using DiscordBot.DAL;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DatabaseContext;Trusted_Connection=True;MultipleActiveResultSets=true",
                    x => x.MigrationsAssembly("DAL.Migrations"));
            });
            var serviceProvider = services.BuildServiceProvider();
            
            var bot = new Bot(serviceProvider);
            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}