using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBooking.Models
{
    public class Flights
    {
        //public Decimal Id { get; set; }
        //public DateTime startdateandtime { get; set; }
        //public DateTime enddateandtime { get; set; }
        //public string fromplace { get; set; }
        //public string toplace { get; set; }
        //public Decimal IsRoundTrip { get; set; }
        //public Decimal price { get; set; }
        //public string AirlineCode { get; set; }
        //public string AirlineName { get; set; }


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
        public string? LogoPath { get; set; }
        public Decimal? IsBlocked { get; set; }
        [NotMapped]
        public string? Logo { get; set; }
        [NotMapped]
        public string? seatsbooked { get; set; }
    }
}
