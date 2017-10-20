using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UrbanScienceCapstone
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?tabs=aspnetcore2x
            //is used to implement session state structure

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //If we try to access session before this call: InvalidOperationException
            app.UseSession(); 
            //might have scalability issue if we dont call "ISession.LoadAsync"
            //keep an eye out for that in the distant future. 

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "login",
                    template: "{controller=Login}/{action=Login}"
                );
                routes.MapRoute(
                    name: "verifylogin",
                    template: "{controller=Home}/{action=verifylogin}"
                );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}");
            });
        }
    }
}
