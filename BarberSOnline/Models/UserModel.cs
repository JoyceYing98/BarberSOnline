﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BarberSOnline.Models
{
    public class UserModel
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Full Name (Same As IC)")]
        [StringLength(60, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Temperature")]
        [Column(TypeName = "decimal(18, 1)")]
        public decimal Temperature { get; set; }

        [Required]
        [Display(Name = "Your Recent Status")]
        public string Status { get; set; }


        [Display(Name = "Your Recent Visit Country/State")]
        public string Visit { get; set; }


    }

}