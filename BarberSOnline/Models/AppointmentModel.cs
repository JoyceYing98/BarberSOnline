using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Models
{
    public class AppointmentModel
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

        [Display(Name = "Check In Date")]
        public DateTime User_Check_In_Date { get; set; }

        [StringLength(200, ErrorMessage = "Cancelled Reason cannot be more than 200 chars")]
        [Display(Name = "Cancelled Reason")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-_,.?!]*$", ErrorMessage = "You can only type alphabets either in lower or upper case, numbers and some symbols such as comma(,), fullstop(.), exclamation mark(!), question mark(?) and etc.")]
        public string User_Cancelled_Reason { get; set; }

        [Display(Name = "ShopEmail")]
        [StringLength(60, MinimumLength = 3)]
        public string ShopEmail { get; set; }//also user

        [Display(Name = "Confirmed Date")]
        public DateTime Shop_Confirmed_Date { get; set; }

        [Display(Name = "Check In Date")]
        public DateTime Shop_Check_In_Date { get; set; }

        [StringLength(200, ErrorMessage = "Cancelled Reason cannot be more than 200 chars")]
        [Display(Name = "Cancelled Reason")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-_,.?!]*$", ErrorMessage = "You can only type alphabets either in lower or upper case, numbers and some symbols such as comma(,), fullstop(.), exclamation mark(!), question mark(?) and etc.")]
        public string Shop_Cancelled_Reason { get; set; }

        [Display(Name = "AdminEmail")]
        [StringLength(60, MinimumLength = 3)]
        public string AdminEmail { get; set; }//also user

        [StringLength(200, ErrorMessage = "Cancelled Reason cannot be more than 200 chars")]
        [Display(Name = "Cancelled Reason")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z0-9""'\s-_,.?!]*$", ErrorMessage = "You can only type alphabets either in lower or upper case, numbers and some symbols such as comma(,), fullstop(.), exclamation mark(!), question mark(?) and etc.")]
        public string Admin_Cancelled_Reason { get; set; }
    }

    public class AppointmentDetailsModel
    {
        public int ID { get; set; }

        [ForeignKey("AppointmentModel")]
        public int AppointmentID { get; set; }

        [Required(ErrorMessage = "Services is required")]
        [StringLength(30, MinimumLength = 3)]
        [Display(Name = "Services")]
        public string Services { get; set; }

        [Required(ErrorMessage = "Appointment Charges is required")]
        [Display(Name = "Charges")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Charges { get; set; }
    }

    public class AppointmentGroupModel
    {
        public AppointmentModel Appointment { get; set; }
        public AppointmentDetailsModel Service { get; set; }

        public List<AppointmentModel> Appointments { get; set; }
        public List<AppointmentDetailsModel> Services { get; set; }
    }
}
