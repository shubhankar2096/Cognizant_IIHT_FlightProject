

namespace AuthServer.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Security.Claims;
    using System.Text;
    using AuthenticationServer;
    using AuthenticationServer.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IOptions<Audience> _settings;

        public AuthController(IOptions<Audience> settings)
        {
            this._settings = settings;
        }

        [HttpPost]
        public IActionResult Post()
        {
            var reader = new StreamReader(Request.Body);
            var msg = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<UserCredentials>(msg);

            var db = new LoginContext();

            var record = db.UserLogin.Find(data.username);

            if (data.username == record.username && data.password == record.password)
            {
                var now = DateTime.UtcNow;

                var claims = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, data.username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
                };

                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    //ValidIssuer = _settings.Value.Iss,
                    ValidateAudience = false,
                   // ValidAudience = _settings.Value.Aud,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,

                };

                var jwt = new JwtSecurityToken(
                    issuer: _settings.Value.Iss,
                    audience: _settings.Value.Aud,
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                var responseJson = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)TimeSpan.FromMinutes(180).TotalSeconds,
                    IsAdmin = (int)record.IsAdmin
                };

                return Json(responseJson);
            }
            else
            {
                return Json("Invalid Username/Password");
            }
        }
    }

    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }
}