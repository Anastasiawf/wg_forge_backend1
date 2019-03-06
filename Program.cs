using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace wg_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
             IWebHost webHost = CreateWebHostBuilder(args).Build();

    using (var scope = webHost.Services.CreateScope())
    {
         // get the IpPolicyStore instance
         var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

         // seed IP data from appsettings
         ipPolicyStore.SeedAsync();
    }

    webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
