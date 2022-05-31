using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBooking.Models
{
    public class FlightDetails
    {
        public Decimal Id { get; set; }
        public Decimal AirlineId { get; set; }
        public string fromplace { get; set; }
        public string toplace { get; set; }
        public DateTime startdatetime { get; set; }
        public DateTime enddatetime { get; set; }
        public string scheduleddays { get; set; }
        public string instrumentused { get; set; }
        public Decimal busiclassseatnum { get; set; }
        public Decimal nonbusiclassseatnum { get; set; }
        public Decimal price { get; set; }
        public Decimal rowsnum { get; set; }
        public string mealtype { get; set; }
        public string flightnumber { get; set; }
        public Decimal IsRoundTrip { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public object? airline { get; set; }
    }
}
