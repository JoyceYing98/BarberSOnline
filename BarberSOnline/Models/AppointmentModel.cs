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

        [Display(Name = "User Email")]
        [StringLength(60, MinimumLength = 3)]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Appointment Type is required")]
        [Display(Name = "Appointment Type")]//On-site service/Shop Service
        public string Type { get; set; }

        [Required(ErrorMessage = "Services is required")]
        [Display(Name = "Services")]
        public string Services { get; set; }//haircut, ...

        [Required(ErrorMessage = "Appointment Charges is required")]
        [Display(Name = "Charges")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Charges { get; set; }

        [Required(ErrorMessage = "Appointment Date is required")]
        [Display(Name = "Appointment Date")]
        public DateTime Appointment_Date { get; set; }

        [Required(ErrorMessage = "Appointment Status is required")]
        //[StringLength(15, MinimumLength = 4)]//Booked, Confirmed, Attended, Paid
        [Display(Name = "Appointment Status")]
        public string Appointment_Status { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [Display(Name = "Barber In-Charged")]
        public string Remark { get; set; }//Barber to serve customer

        [Display(Name = "Appointment Created Date")]
        public DateTime User_Booked_Date { get; set; }

        [Display(Name = "Appointment Confirmed Date")]
        public DateTime User_Confirmed_Date { get; set; }

        [StringLength(200, ErrorMessage = "Cancelled Reason cannot be more than 200 chars")]
        [Display(Name = "User Cancelled Reason")]
        [RegularExpression(@"^[a-zA-Z0-9""'\s-_,.?!]*$", ErrorMessage = "You can only type alphabets either in lower or upper case, numbers and some symbols such as comma(,), fullstop(.), exclamation mark(!), question mark(?) and etc.")]
        public string User_Cancelled_Reason { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime Barber_Approved_Date { get; set; }

        [Display(Name = "Check In Date")]
        public DateTime Barber_Check_In_Date { get; set; }

        [Display(Name = "Barber Email")]
        [StringLength(60, MinimumLength = 3)]
        public string BarberEmail { get; set; }//also user

        [StringLength(200, ErrorMessage = "Cancelled Reason cannot be more than 200 chars")]
        [Display(Name = "Barber Cancelled Reason")]
        [RegularExpression(@"^[a-zA-Z0-9""'\s-_,.?!]*$", ErrorMessage = "You can only type alphabets either in lower or upper case, numbers and some symbols such as comma(,), fullstop(.), exclamation mark(!), question mark(?) and etc.")]
        public string Barber_Cancelled_Reason { get; set; }

        [Display(Name = "Admin Email")]
        [StringLength(60, MinimumLength = 3)]
        public string AdminEmail { get; set; }//also user

        [StringLength(200, ErrorMessage = "Cancelled Reason cannot be more than 200 chars")]
        [Display(Name = "Admin Cancelled Reason")]
        [RegularExpression(@"^[a-zA-Z0-9""'\s-_,.?!]*$", ErrorMessage = "You can only type alphabets either in lower or upper case, numbers and some symbols such as comma(,), fullstop(.), exclamation mark(!), question mark(?) and etc.")]
        public string Admin_Cancelled_Reason { get; set; }
    }
}
