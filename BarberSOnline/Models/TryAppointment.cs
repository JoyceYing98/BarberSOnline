using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Models
{
    public class TryAppointment
    {
        public int ID { get; set; }

        [Display(Name = "UserEmail")]
        [StringLength(60, MinimumLength = 3)]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Appointment Status is required")]
        [StringLength(15, MinimumLength = 6)]//Booked, Confirmed, Attended, Paid
        [Display(Name = "Appointment Status")]
        public string Status { get; set; }

        [Display(Name = "Confirmed Date")]
        public DateTime User_Confirmed_Date { get; set; }
    }
}
