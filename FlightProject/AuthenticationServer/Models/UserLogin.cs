using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models
{
    public class UserLogin
    {
        [Key]
        public string username { get; set; }
        public string password { get; set; }
        public Decimal IsAdmin { get; set; }
    }
}
