using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlightBooking
{
    class QueueConsumer
    {
        public static void ConsumeIsBlocked(IModel channel)
        {
            channel.QueueDeclare("flightapp-queue",
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null
               );
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = Encoding.UTF8.GetString(e.Body.ToArray());
                var data = JsonConvert.DeserializeObject<Dictionary<string,string>>(body);
                try
                {
                    var db = new FlightBookingContext();

                   var data1 = (from f in db.Flights
                                where f.AirlineId == Convert.ToDecimal(data["Id"])
                                select f).ToList();
                    foreach(var row in data1)
                    {
                        row.IsBlocked = Convert.ToDecimal(data["IsBlocked"]);
                    }
                    //db.Update();
                    db.SaveChanges();
                }
                catch (Exception e1)
                {
                }
            };

            channel.BasicConsume("flightapp-queue", true, consumer);

        }
    }
}
