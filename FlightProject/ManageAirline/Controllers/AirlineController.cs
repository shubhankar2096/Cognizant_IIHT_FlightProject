using Grpc.Core;
using ManageAirline.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace ManageAirline.Controllers
{
    [Route("api/airline")]
    [ApiController]
    public class AirlineController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AirlineController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        // GET api/values
        [HttpPost]
        [Route("addairline")]
        public string AddAirline()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var msg = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<AirlineDetails>(msg);
                var db = new ManageAirlineContext();
                Decimal Id = db.Airline.Max(p => p.Id) + 1;

                string LogoImage = data.LogoImage;
                var parent = Directory.GetParent(Directory.GetCurrentDirectory());
                string location = parent.FullName + "\\Images\\" + data.Name + ".png";
                byte[] imageBytes = Encoding.UTF8.GetBytes(LogoImage);

                FileStream fs = new FileStream(location, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                try
                {
                    bw.Write(imageBytes);
                }
                finally
                {
                    fs.Close();
                    bw.Close();
                }

                var record = new Airline { Id = Id, Code = data.Code, Name = data.Name, contactnum = data.contactnum, contactaddr = data.contactaddr, LogoPath = location };
                db.Airline.AddAsync(record);
                db.SaveChanges();
                return JsonConvert.SerializeObject("success");
            }
            catch (Exception e)
            {
                return "error";
            }
        }

        [HttpGet]
        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [Route("getairlines")]
        public object GetAirlines()
        {

            var db = new ManageAirlineContext();

            var data = (from a in db.Airline
                        select a).ToList();


            foreach (var record in data)
            {
                var location = record.LogoPath;
                if (location == null)
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
            }
            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        [Route("blockairline")]
        public string BlockAirline()
        {
            try
            {
                Decimal Id = Convert.ToDecimal(Request.Query["Id"]);
                var db = new ManageAirlineContext();
                var record = db.Airline.Single(u => u.Id == Id);
                record.IsBlocked = 1;
                db.SaveChanges();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9100/api/flights/blockairline");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");
                //request.Content = JsonContent.Create(record);

                var client = _httpClientFactory.CreateClient();
                client.SendAsync(request);


                var factory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672")
                };

                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                QueProducer.PublishIsBlocked(channel, record);

                return JsonConvert.SerializeObject("success");
            }
            catch (Exception e)
            {
                return "error";
            }

        }
        [HttpPost]
        [Route("scheduleflight")]
        public string ScheduleFlight()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var msg = reader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<FlightInventoryDetails>(msg);
                var db = new ManageAirlineContext();
                Decimal Id = db.FlightInventory.Max(p => p.Id) + 1;

                string flightno = "F" + "0000000000";
                flightno = flightno.Remove(flightno.Length - 1 - Id.ToString().Length, Id.ToString().Length);
                flightno = flightno + Id;

                var record = new FlightInventory { Id = Id, AirlineId = data.AirlineId, fromplace = data.fromplace, toplace = data.toplace, scheduleddays = data.scheduleddays, instrumentused = data.instrumentused, busiclassseatnum = data.busiclassseatnum, nonbusiclassseatnum = data.nonbusiclassseatnum, price = data.price, rowsnum = data.rowsnum, mealtype = data.mealtype, flightnumber = flightno, startdatetime = Convert.ToDateTime(data.startdatetime), enddatetime = Convert.ToDateTime(data.enddatetime), IsRoundTrip = data.IsRoundTrip };
                db.FlightInventory.AddAsync(record);
                db.SaveChanges();

                var airline = db.Airline.Find(data.AirlineId);

                record.AirlineCode = airline.Code;
                record.AirlineName = airline.Name;
                record.LogoPath = airline.LogoPath;
                record.airline = null;

                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9100/api/flights/addflight");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "HttpClientFactory-Sample");
                request.Content = JsonContent.Create(record);

                var client = _httpClientFactory.CreateClient();
                client.SendAsync(request);

                return JsonConvert.SerializeObject(flightno);
            }
            catch (Exception e)
            {
                return "error";
            }
        }


        [HttpGet]
        [Route("getflightinventory")]
        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public object GetFlightInventory()
        {
            string airlinecode = Request.Query["airlinecode"];
            string flightnum = Request.Query["flightnum"];
            string instrumentused = Request.Query["instrumentused"];

            var db = new ManageAirlineContext();

            var data = (from f in db.FlightInventory join a in db.Airline on f.AirlineId equals a.Id
                        where airlinecode != "" && a.Code != null ? a.Code == airlinecode : true
                        where flightnum != "" && f.flightnumber != null ? f.flightnumber == flightnum : true
                        where instrumentused != "" && f.instrumentused != null ? f.instrumentused == instrumentused : true
                        select new { f, a }
                        ).ToList();


            /*foreach (var record in data)
            {
                var location = record.a.LogoPath;
                if (location == null)
                {
                    continue;
                }
                FileStream fs = new FileStream(location, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] logo = br.ReadBytes(1000000000);
                string logoimage = Convert.ToBase64String(logo);
                record.a.Logo = "data:image/jpeg;base64," + logoimage;
                fs.Close();
                br.Close();
            }*/
            return JsonConvert.SerializeObject(data);
        }
        // GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
