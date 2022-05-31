using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageAirline
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            //Configuration = configuration;
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            //services.AddHttpClient("flights", f =>
            //{
            //    f.BaseAddress = new Uri("http://localhost:9100/");
            //    f.DefaultRequestHeaders.Add("Accept", "application/json");
            //    f.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            //});
            services.AddHttpClient();
            services.AddAuthentication().AddJwtBearer();

            services.AddCors(options =>
               options.AddPolicy(name: "angularApplication", (builder) =>
               {
                   builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
               })
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("angularApplication");
            //app.UseMvc();
            app.UseMiddleware<JwtMiddleware>();
            //app.MapWhen(
            //        httpContext => !httpContext.Request.Path.StartsWithSegments("/flights"),
            //        subApp => subApp.UseMiddleware<JwtMiddleware>()
            //    );

            app.UseRouting();
            //app.UseCors();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseAuthentication();

            //app.UseCors("_myAllowSpecificOrigins");

            //app.UseHttpsRedirection();

            //app.UseAuthorization();
        }
    }
    
}
