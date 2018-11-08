using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Stratis.Guru.Hubs;
using Stratis.Guru.Models;
using Stratis.Guru.Modules;
using Stratis.Guru.Services;
using Stratis.Guru.Settings;
using static Microsoft.AspNetCore.Http.SameSiteMode;

namespace Stratis.Guru
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = None;
            });
            services.Configure<NakoApiSettings>(Configuration.GetSection("NakoApi"));
            services.Configure<FixerApiSettings>(Configuration.GetSection("FixerApi"));
            
            /*Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");*/
            
            services.AddMemoryCache();
            
            services.AddTransient<UpdateHub>();
            services.AddSingleton<IAsk, Ask>();

            services.AddHostedService<UpdateInfosService>();
            services.AddHostedService<FixerService>();
            services.AddHostedService<VanityService>();
            
            services.AddLocalization();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Node.js Modules Directory Access
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "node_modules")
                ),
                RequestPath = "/npm"
            });
            
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Documentation/site/")),
                RequestPath = "/documentation",
                EnableDirectoryBrowsing = false,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = {"index.html"}}
            });
            
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("fr-FR"),
                new CultureInfo("ru"),
                new CultureInfo("it"),
                new CultureInfo("de"),
                new CultureInfo("cn")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            
            app.UseSignalR(routes =>
            {
                routes.MapHub<UpdateHub>("/update");
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}