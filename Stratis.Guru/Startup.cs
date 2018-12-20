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
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            
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
            
            services.AddMemoryCache();
            
            services.AddTransient<UpdateHub>();
            services.AddSingleton<IAsk, Ask>();

            services.AddHostedService<UpdateInfosService>();
            services.AddHostedService<FixerService>();
            services.AddHostedService<VanityService>();
            
            services.AddLocalization();
            
            services.AddMvc();
            
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
            
            // Add Documentation (MkDocs) Support
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Documentation/site/")),
                RequestPath = "/documentation",
                EnableDirectoryBrowsing = false,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = {"index.html"}}
            });

            // Add Culture Detection Support
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => !x.IsNeutralCulture).ToList();
            var defaultCulture = new RequestCulture("en-US");
            defaultCulture.UICulture.NumberFormat.CurrencySymbol = "$";
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = defaultCulture,
                SupportedCultures = allCultures,
                SupportedUICultures = allCultures
            });
            
            // Add SignalR support for automatically update ticker price
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