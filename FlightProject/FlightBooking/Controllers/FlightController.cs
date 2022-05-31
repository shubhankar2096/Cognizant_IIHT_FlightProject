using FlightBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace FlightBooking.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public object GetFlights()
        {
            DateTime startdateandtime = Request.Query["startdatetime"] == "null" ? DateTime.MinValue : Convert.ToDateTime(Request.Query["startdatetime"]);
            DateTime enddateandtime = Request.Query["enddatetime"] == "null" ? DateTime.MinValue : Convert.ToDateTime(Request.Query["enddatetime"]);
            string fromplace = Request.Query["fromplace"];
            string toplace = Request.Query["toplace"];
            string isroundtrip = Request.Query["isroundtrip"];

            var db = new FlightBookingContext();

             var data = (from f in db.Flights
                           where (startdateandtime != DateTime.MinValue ? f.startdatetime.ToShortDateString().Equals(startdateandtime.ToShortDateString()) : true)
                           where (enddateandtime != DateTime.MinValue ? f.enddatetime.ToShortDateString().Equals(enddateandtime.ToShortDateString()) : true)
                           where fromplace != null ? f.fromplace.Equals(fromplace) : true
                           where toplace != null ? f.toplace.Equals(toplace) : true
                           where isroundtrip != "null" && isroundtrip != "false" ? f.IsRoundTrip.Equals(isroundtrip == "true" ? 1 : 0) : true
                           where f.IsBlocked != 1 
                           select f).ToList();


            foreach(var record in data)
            {
                var location = record.LogoPath;
                if(location == null)
                {
                    continue;
                }
                FileStream fs = new FileStream(location, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] logo = br.ReadBytes(1000000000);
                string logoimage = Convert.ToBase64String(logo);
                record.Logo = "data:image/jpeg;base64," + logoimage;
                fs.Close();
                br.Close();

                var seatsbooked = (from u in db.UserBooking
                                   where u.FlightId == record.Id
                                   select u.seatsbooked
                    ).ToList();
                string seatsbookedlist = "";
                foreach(var item in seatsbooked)
                {
                    seatsbookedlist = seatsbookedlist + "," + item;
                }

                if (seatsbookedlist.Length > 0)
                {
                    seatsbookedlist = seatsbookedlist.Remove(0, 1);
                }

                record.seatsbooked = seatsbookedlist;
            }
            return JsonConvert.SerializeObject(data);
        }

        // GET api/values/
        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{useremail}")]
        public string GetUserBookingHistory(string useremail)
        {
            var db = new FlightBookingContext();

            var data = (from u in db.UserBooking
                        join f in db.Flights on u.FlightId equals f.Id
                        where u.useremail.Equals(useremail)
                        orderby u.ticketnumber descending
                        select new
                        {
                            f,
                            u
                        }).ToList();
            foreach (var record in data)
            {
                var location = record.f.LogoPath;
                if (location == null)
                {
                    continue;
                }
                FileStream fs = new FileStream(location, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] logo = br.ReadBytes(1000000000);
                string logoimage = Convert.ToBase64String(logo);
                record.f.Logo = "data:image/jpeg;base64," + logoimage;
                fs.Close();
                br.Close();
            }

                return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        [Route("getticketdetails/{ticketno}")]
        public string GetTicketDetails(string ticketno)
        {
            var db = new FlightBookingContext();

            var data = (from u in db.UserBooking
                        join f in db.Flights on u.FlightId equals f.Id
                        where u.ticketnumber.Equals(ticketno)
                        select new
                        {
                            f,
                            u
                        }).ToList();

            return JsonConvert.SerializeObject(data);
        }

        // POST api/values
        [HttpPost]
        [Route("bookflight")]
        public string BookFlight()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var msg = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<UserDetails>(msg);
                var db = new FlightBookingContext();
                Decimal ticketid = db.UserBooking.Max(p => p.Id) + 1;

                string ticketno = "TN" + "0000000000";
                ticketno = ticketno.Remove(ticketno.Length - 1 - ticketid.ToString().Length, ticketid.ToString().Length);
                ticketno = ticketno + ticketid;

                data.passengerdetails = data.passengerdetails.Remove(0, 1);

                var record = new UserBooking { Id = ticketid, username = data.username, useremail = data.useremail, numberofseats = data.numberofseats, passengerdetails = data.passengerdetails, mealtype = data.mealtype, seatsbooked = data.seatsbooked, FlightId = data.FlightId, ticketnumber = ticketno, price=data.price };
                db.UserBooking.AddAsync(record);
                db.SaveChanges();

                //return ticketno;
                return JsonConvert.SerializeObject(ticketno);
            }
            catch(Exception e)
            {
                return "error";
            }
        }

        // PUT api/values/5
        [HttpGet]
        [Route("cancelflight")]
        public string CancelFlight()
        {
            try
            {
                string ticketno = Request.Query["ticketnumber"];
                var db = new FlightBookingContext();
                var record = db.UserBooking.Single(u=>u.ticketnumber.Equals(ticketno));
                record.IsCanceled = 1;
                db.SaveChanges();
                return JsonConvert.SerializeObject("success");
            }
            catch (Exception e)
            {
                return "error";
            }

        }
        [HttpPost]
        [Route("addflight")]
        public void ScheduleFlight()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var msg = reader.ReadToEnd();
                //var data = JsonConvert.DeserializeObject<FlightDetails>(msg);
                var data = JsonConvert.DeserializeObject<Dictionary<string,string>>(msg);
                var db = new FlightBookingContext();
                Decimal Id = db.Flights.Max(p => p.Id) + 1;

                var record = new Flights { Id = Id, AirlineId = Convert.ToDecimal(data["airlineId"]),  fromplace = data["fromplace"], toplace = data["toplace"], scheduleddays = data["scheduleddays"], instrumentused = data["instrumentused"], busiclassseatnum = Convert.ToDecimal(data["busiclassseatnum"]), nonbusiclassseatnum = Convert.ToDecimal(data["nonbusiclassseatnum"]), price = Convert.ToDecimal(data["price"]), rowsnum = Convert.ToDecimal(data["rowsnum"]), mealtype = data["mealtype"], flightnumber = data["flightnumber"], startdatetime = Convert.ToDateTime(data["startdatetime"]), enddatetime = Convert.ToDateTime(data["enddatetime"]), IsRoundTrip = Convert.ToDecimal(data["isRoundTrip"]), AirlineCode = data["airlineCode"], AirlineName=data["airlineName"], LogoPath = data["logoPath"] };
                db.Flights.AddAsync(record);
                db.SaveChanges();

            }
            catch (Exception e)
            {

            }
        }


        /*[HttpPost]
        [Route("blockairline")]
        public string BlockAirline()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var msg = reader.ReadToEnd();
                //var data = JsonConvert.DeserializeObject<FlightDetails>(msg);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg);
                var db = new FlightBookingContext();

                var record = new AirlineBlocked { AirlineId = Convert.ToDecimal(data["AirlineId"]), IsBlocked = Convert.ToDecimal(data["IsBlocked"])};

                db.AirlineBlocked.AddAsync(record);
                db.SaveChanges();

                return "success";
            }
            catch (Exception e)
            {
                return "error";
            }
        }*/

        [HttpPost]
        [Route("blockairline")]
        public void BlockAirline()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672")
                };

                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                QueueConsumer.ConsumeIsBlocked(channel);
            }
            catch (Exception e)
            {
            }
        }
    }
}
