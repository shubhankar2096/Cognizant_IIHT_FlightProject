using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBooking.Models
{
    public class AirlineBlocked
    {
        public Decimal AirlineId { get; set; }
        public Decimal IsBlocked { get; set; }
    }
}
