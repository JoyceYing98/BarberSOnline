using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BarberSOnline.Models;

namespace BarberSOnline.Data
{
    public class BarberSOnlineContent : DbContext
    {
        public BarberSOnlineContent (DbContextOptions<BarberSOnlineContent> options)
            : base(options)
        {
        }

        public DbSet<BarberSOnline.Models.UserModel> UserModel { get; set; }
    }
}
