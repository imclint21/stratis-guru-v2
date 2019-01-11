using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Stratis.Guru.Hubs;
using Stratis.Guru.Models;
using Stratis.Guru.Modules;
using Stratis.Guru.Services;
using Stratis.Guru.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            services.AddCors();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = None;
            });

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.Configure<NakoSettings>(Configuration.GetSection("Nako"));
            services.Configure<DrawSettings>(Configuration.GetSection("Draw"));
            services.Configure<CurrencySettings>(Configuration.GetSection("Currency"));
            services.Configure<TickerSettings>(Configuration.GetSection("Ticker"));
            services.Configure<SetupSettings>(Configuration.GetSection("Setup"));
            services.Configure<FeaturesSettings>(Configuration.GetSection("Features"));

            services.AddMemoryCache();

            services.AddTransient<UpdateHub>();
            services.AddSingleton<IAsk, Ask>();
            services.AddTransient<DatabaseContext>();
            services.AddSingleton<ISettings, Models.Settings>();
            services.AddSingleton<IDraws, Draws>();
            services.AddSingleton<IParticipation, Participations>();
            services.AddSingleton<BlockIndexService>();
            services.AddSingleton<TickerService>();
            services.AddSingleton<CurrencyService>();

            services.AddHostedService<DataUpdateService>();
            services.AddHostedService<LotteryService>();
            services.AddHostedService<VanityService>();

            services.AddLocalization();

            services.AddMvc();

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = "6LfmOIQUAAAAAIEsH2nG6kEiL-bpLhvm0ibhHnol",    //Configuration["Recaptcha:SiteKey"],
                SecretKey = "6LfmOIQUAAAAAO06PpD8MmndjrjfBr7x-fgnDt2G"  //Configuration["Recaptcha:SecretKey"]
            });

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
                app.UseHttpsRedirection();
            }

            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseCors(builder => builder.WithOrigins("http:/localhost:1989"));

            // Add Documentation (MkDocs) Support
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Documentation/site/")),
                RequestPath = "/documentation",
                EnableDirectoryBrowsing = false,
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = { "index.html" } }
            });

            // Add Culture Detection Support
            List<CultureInfo> allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => !x.IsNeutralCulture).ToList();
            RequestCulture defaultCulture = new RequestCulture("en-US");
            defaultCulture.UICulture.NumberFormat.CurrencySymbol = "$";

            // Add some known cultures that doesn't parse in Chrome/Firefox.
            allCultures.Add(new CultureInfo("en"));
            allCultures.Add(new CultureInfo("no"));
            allCultures.Add(new CultureInfo("nb"));

            RequestLocalizationOptions requestOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = defaultCulture,
                SupportedCultures = allCultures,
                SupportedUICultures = allCultures
            };

            requestOptions.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider { Options = requestOptions },
                new CookieRequestCultureProvider { Options = requestOptions },
                new AcceptLanguageHeaderRequestCultureProvider { Options = requestOptions }
            };
            
            app.UseRequestLocalization(requestOptions);

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