using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBooking.Models
{
    public class UserDetails
    {
        public string username { get; set; }
        public string useremail { get; set; }
        public Decimal numberofseats { get; set; }
        public string passengerdetails { get; set; }
        public string mealtype { get; set; }
        public string seatsbooked { get; set; }
        public Decimal FlightId { get; set; }
        public Decimal price { get; set; }

    }
}
