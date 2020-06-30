using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Stratis.Guru.Hubs;
using Stratis.Guru.Models;
using Stratis.Guru.Modules;
using Stratis.Guru.Services;
using Stratis.Guru.Settings;

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
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
            });

            services.Configure<NakoApiSettings>(Configuration.GetSection("NakoApi"));
            services.Configure<FixerApiSettings>(Configuration.GetSection("FixerApi"));
            services.Configure<DrawSettings>(Configuration.GetSection("Draw"));
            services.Configure<TickerSettings>(Configuration.GetSection("Ticker"));
            services.Configure<SetupSettings>(Configuration.GetSection("Setup"));
            services.Configure<FeaturesSettings>(Configuration.GetSection("Features"));
            services.Configure<ColdStakingSettings>(Configuration.GetSection("ColdStaking"));

            services.AddMemoryCache();

            services.AddTransient<UpdateHub>();
            services.AddSingleton<IAsk, Ask>();
            //services.AddTransient<DatabaseContext>();
            //services.AddSingleton<ISettings, Models.Settings>();
            //services.AddSingleton<IDraws, Draws>();
            //services.AddSingleton<IParticipation, Participations>();
            services.AddSingleton<BlockIndexService>();

            services.AddHostedService<UpdateInfosService>();
            services.AddHostedService<FixerService>();
            //services.AddHostedService<LotteryService>();
            services.AddHostedService<VanityService>();

            services.AddLocalization();

            services.AddControllersWithViews();

            services.AddRecaptcha(new RecaptchaOptions
            {
                SiteKey = "6LfmOIQUAAAAAIEsH2nG6kEiL-bpLhvm0ibhHnol",    //Configuration["Recaptcha:SiteKey"],
                SecretKey = "6LfmOIQUAAAAAO06PpD8MmndjrjfBr7x-fgnDt2G"  //Configuration["Recaptcha:SecretKey"]
            });

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();   //.Where(x => !x.IsNeutralCulture)
            var defaultCulture = new RequestCulture("en-US");
            defaultCulture.UICulture.NumberFormat.CurrencySymbol = "$";
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = defaultCulture,
                SupportedCultures = allCultures,
                SupportedUICultures = allCultures
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<UpdateHub>("/update");
            });

        }
    }
}
