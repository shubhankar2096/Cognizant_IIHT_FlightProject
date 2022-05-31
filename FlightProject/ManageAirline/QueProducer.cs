using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManageAirline
{
    class QueProducer
    {
        public static void PublishIsBlocked(IModel channel, Models.Airline record)
        {
            channel.QueueDeclare("flightapp-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );
            //int count = 0;

            //var message = new { Name = "Team Batch 21", Message = $"Hello Team Count: {count}" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record));
            //var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record));
            channel.BasicPublish("", "flightapp-queue", null, body);

        }
    }
}
