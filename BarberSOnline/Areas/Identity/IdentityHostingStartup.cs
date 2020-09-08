using System;
using BarberSOnline.Areas.Identity.Data;
using BarberSOnline.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BarberSOnline.Areas.Identity.IdentityHostingStartup))]
namespace BarberSOnline.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<BarberSOnlineContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("BarberSOnlineContextConnection")));

                services.AddIdentity<BarberSOnlineUser, IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                 .AddDefaultUI()
                 .AddEntityFrameworkStores<BarberSOnlineContext>()
                 .AddDefaultTokenProviders();

                
            });
            }
    }
 }