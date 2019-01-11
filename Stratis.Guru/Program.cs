using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Stratis.Guru
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var chain = (args.Length == 0) ? "STRAT" : args[0].ToUpper();

            var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("hosting.json", optional: true)
              .AddJsonFile("setup.json", optional: false, reloadOnChange: false)
              .AddJsonFile(Path.Combine("Setup", $"{chain}.json"), optional: false, reloadOnChange: false)
              .AddCommandLine(args)
              .AddEnvironmentVariables()
              .Build();

            WebHost.CreateDefaultBuilder(args)
               .UseConfiguration(config)
               .UseStartup<Startup>()
               .Build().Run();
        }
    }
}