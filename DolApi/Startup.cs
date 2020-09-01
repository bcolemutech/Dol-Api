using AspNetCore.Firebase.Authentication.Extensions;
using DolApi.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DolApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddFirebaseAuthentication(Configuration["FirebaseAuthentication:Issuer"],
                Configuration["FirebaseAuthentication:Audience"]);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Authority", "1"));
                options.AddPolicy("Testers", policy => policy.RequireClaim("Authority", "1", "2"));
                options.AddPolicy("Players", policy => policy.RequireClaim("Authority", "1", "2", "3"));
            });
            
            services.AddSingleton<IUserController, UserController>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMvc();
        }
    }
}
