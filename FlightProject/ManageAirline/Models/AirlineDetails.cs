using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageAirline.Models
{
    public class AirlineDetails
    {
        public Decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? LogoImage { get; set; }
        public string contactnum { get; set; }
        public string contactaddr { get; set; }
    }
}
