using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ManageAirline.Models
{
    public class Airline
    {
        public Decimal Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? LogoPath { get; set; }
        public Decimal IsBlocked { get; set; }
        public string contactnum { get; set; }
        public string contactaddr { get; set; }
        [NotMapped]
        public string? Logo { get; set; }
    }
}
