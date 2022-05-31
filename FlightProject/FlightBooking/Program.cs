using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore;
using FlightBooking.Models;
using System.Configuration;
using FlightBooking;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FlightBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //using (var db = new FlightBookingContext())
            //{
            //    var flight1 = new Flights { Id = 1, dateandtime = new DateTime(2022, 05, 13, 12, 00, 00), fromplace = "Mumbai", toplace= "Berlin", IsRoundTrip=true, price =100000 };
            //    db.Flights.AddAsync(flight1);

            //    var flight2 = new Flights { Id = 2, dateandtime = new DateTime(2022, 05, 13, 08, 00, 00), fromplace = "Delhi", toplace = "Sydney", IsRoundTrip = false, price = 150000 };
            //    db.Flights.AddAsync(flight2);

            //    var flight3 = new Flights { Id = 3, dateandtime = new DateTime(2022, 05, 13, 12, 00, 00), fromplace = "Mumbai", toplace = "Amsterdam", IsRoundTrip = true, price = 170000 };
            //    db.Flights.AddAsync(flight3);

            //    var flight4 = new Flights { Id = 4, dateandtime = new DateTime(2022, 05, 14, 07, 00, 00), fromplace = "Delhi", toplace = "Berlin", IsRoundTrip = false, price = 100000 };
            //    db.Flights.AddAsync(flight4);

            //    var flight5 = new Flights { Id = 5, dateandtime = new DateTime(2022, 05, 14, 14, 00, 00), fromplace = "Sydney", toplace = "Mumbai", IsRoundTrip = true, price = 120000 };
            //    db.Flights.AddAsync(flight5);

            //    db.SaveChanges();
            //}

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            QueueConsumer.ConsumeIsBlocked(channel);

            /*var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var queueDeclareResponse = channel.QueueDeclare("flightapp-queue", false, false, false, null);

                    var consumer = new QueueConsumer(channel);
                    //channel.BasicConsume("flightapp-queue", true, consumer);

                    for (int i = 0; i < queueDeclareResponse.MessageCount; i++)
                    {
                        consumer.ConsumeIsBlocked();
                    }
                }
            }*/

                CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:9100/");
                //.UseRequestCulture();
    }

    public class FlightBookingContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (options.IsConfigured == false)
            {

                //var Conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings");
                options.UseSqlServer("Server=CTSDOTNET147;Database=FlightBookingDB;Trusted_Connection=false;User ID=sa;Password=pass@word1;MultipleActiveResultSets=true;Connection Timeout=3000");
                //options.UseSqlServer(Conn.Value);
                
            }
        }

        public DbSet<Models.Flights> Flights { get; set; }
        public DbSet<Models.UserBooking> UserBooking { get; set; }
        //public DbSet<Models.AirlineBlocked> AirlineBlocked { get; set; }
    }
}
