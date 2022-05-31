using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManageAirline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:9300/");
    }


    public class ManageAirlineContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (options.IsConfigured == false)
            {
                options.UseSqlServer("Server=CTSDOTNET147;Database=AirlineDB;Trusted_Connection=false;User ID=sa;Password=pass@word1;MultipleActiveResultSets=true;Connection Timeout=3000");
            }
        }

        public DbSet<Models.Airline> Airline { get; set; }
        public DbSet<Models.FlightInventory> FlightInventory { get; set; }
    }
}
