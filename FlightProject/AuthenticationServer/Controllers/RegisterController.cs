using AuthenticationServer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Controllers
{
    public class RegisterController : Controller
    {
        [Route("api/auth/register")]
        [HttpPost]
        public string Register()
        {
            var reader = new StreamReader(Request.Body);
            var msg = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<UserCredentials>(msg);

            var db = new LoginContext();

            var record = new UserLogin { username = data.username, password = data.password, IsAdmin = 0 };
            db.UserLogin.Add(record);
            db.SaveChanges();

            return JsonConvert.SerializeObject("success");
        }
    }
}
