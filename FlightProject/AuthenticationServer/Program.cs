using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthenticationServer
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
                 .UseUrls("http://localhost:9400/");
    }

    public class LoginContext : Microsoft.EntityFrameworkCore.DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (options.IsConfigured == false)
            {
                options.UseSqlServer("Server=CTSDOTNET147;Database=LoginDB;Trusted_Connection=false;User ID=sa;Password=pass@word1;MultipleActiveResultSets=true;Connection Timeout=3000");
            }
        }

        public Microsoft.EntityFrameworkCore.DbSet<Models.UserLogin> UserLogin { get; set; }
    }
}
