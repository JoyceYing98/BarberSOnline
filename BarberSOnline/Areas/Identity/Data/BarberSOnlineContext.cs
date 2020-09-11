using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarberSOnline.Data
{
    public class BarberSOnlineContext : IdentityDbContext<BarberSOnlineUser>
    {
        public BarberSOnlineContext(DbContextOptions<BarberSOnlineContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<BarberSOnline.Models.UserModel> UserModel { get; set; }
        public DbSet<BarberSOnline.Models.AppointmentModel> AppointmentModel { get; set; }
        public DbSet<BarberSOnline.Models.AppointmentDetailsModel> AppointmentDetailsModel { get; set; }

    }
}
