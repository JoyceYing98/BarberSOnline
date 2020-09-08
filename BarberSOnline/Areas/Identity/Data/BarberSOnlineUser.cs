using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BarberSOnline.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the BarberSOnlineUser class
    public class BarberSOnlineUser : IdentityUser
    {
        public byte[] ProfileImage { get; set; }

        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }
        [PersonalData]
        public string Address { get; set; }
    }
}
