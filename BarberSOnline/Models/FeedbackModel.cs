using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Models
{
    public class FeedbackModel : TableEntity
    {
        public FeedbackModel()
        {

        }
        public FeedbackModel(int fId, string email)
        {
            this.RowKey = fId.ToString();
            this.PartitionKey = email;
        }


        public int feedbackId { get; set; }
        
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        public DateTime feedDate { get; set; }

        [Required(ErrorMessage = "Feedback Title is required")]
        [Display(Name = "Title")]
        public string FeedbackTitle { get; set; }

        [Required(ErrorMessage = "Feedback Content is required")]
        [Display(Name = "Content")]
        public string FeedbackContent { get; set; }

        [Required(ErrorMessage = "Rating is Required")]
        [Display(Name = "Rating")]
        public string Rating { get; set; }
    }
}
