using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Models
{
    public class ProfileModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateofBirth { get; set; }

        public string Address { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
