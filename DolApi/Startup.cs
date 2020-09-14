using System;
using System.Diagnostics.CodeAnalysis;
using AspNetCore.Firebase.Authentication.Extensions;
using DolApi.Repositories;
using DolApi.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DolApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            AppOptions options = new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = Configuration["ProjectId"]
            };
            
            var app = FirebaseApp.Create(options);
            Console.WriteLine($"My app ID is {app.Options.ProjectId}!");
            
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddFirebaseAuthentication(Configuration["FirebaseAuthentication:Issuer"],
                Configuration["FirebaseAuthentication:Audience"]);

            services.AddAuthorization(option =>
            {
                option.AddPolicy("Admin", policy => policy.RequireClaim("Authority", "0"));
                option.AddPolicy("Testers", policy => policy.RequireClaim("Authority", "0", "1"));
                option.AddPolicy("Players", policy => policy.RequireClaim("Authority", "0", "1", "2"));
            });
            
            services.AddHttpContextAccessor();

            services.AddSingleton<IAdminService, AdminService>();
            services.AddSingleton<ICharacterRepo, CharacterRepo>();
            services.AddSingleton<IPlayerRepo, PlayerRepo>();
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
