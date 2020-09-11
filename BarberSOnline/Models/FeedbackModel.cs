using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Models
{
    public class FeedbackModel : TableEntity
    {
        public FeedbackModel()
        {

        }

        public FeedbackModel(int fid, string FeedbackTitle)
        {
            this.RowKey = fid.ToString();
            this.PartitionKey = FeedbackTitle;
        }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Feedback Content is Required")]
        [StringLength(255, MinimumLength = 3)]
        [Display(Name = "Feedback Content")]
        public string FeedbackContent { get; set; }

        [Required(ErrorMessage = "Rating is Required")]
        [Display(Name = "Rating")]
        public string Rating { get; set; }
    }
}
