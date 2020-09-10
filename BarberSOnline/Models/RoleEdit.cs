using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberSOnline.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BarberSOnline.Models
{
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<BarberSOnlineUser> Members { get; set; }
        public IEnumerable<BarberSOnlineUser> NonMembers { get; set; }
    }
}
