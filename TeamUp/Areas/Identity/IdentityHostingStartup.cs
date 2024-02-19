using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamUp.Areas.Identity.Data;
using TeamUp.Data;

[assembly: HostingStartup(typeof(TeamUp.Areas.Identity.IdentityHostingStartup))]
namespace TeamUp.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TeamUpDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TeamUpDbContextConnection")));

                services.AddDefaultIdentity<TeamUpUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
                    .AddEntityFrameworkStores<TeamUpDbContext>();
            });
        }
    }
}