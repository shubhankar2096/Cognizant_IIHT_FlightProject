using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageAirline
{
    public class JwtMiddleware
    {
        RequestDelegate _next;
        Models.AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<Models.AppSettings> appSettings)
        {
            _next = next;
            //_appSettings = appSettings.Value;

            using (StreamReader r = new StreamReader("./appsettings.json"))
            {
                string data = r.ReadToEnd();
                _appSettings = JsonConvert.DeserializeObject<Models.AppSettings>(data);
            }
        }

        public async Task Invoke(HttpContext context)
        {

            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var token = context.Request.Headers["Authorization"].ToString();

            if (token != null && token != "")
                attachUserToContext(context, token);

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, string token)
        {
            try
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                //var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                //context.Items["User"] = userService.GetById(userId);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }

    //public static class RequesteMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseRequestCulture(
    //        this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<JwtMiddleware>();
    //    }
    //}
}