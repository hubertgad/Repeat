using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Repeat.Areas.Identity.IdentityHostingStartup))]
namespace Repeat.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}