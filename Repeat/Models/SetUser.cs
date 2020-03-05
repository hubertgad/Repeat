﻿using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class SetUser
    {
        [Required]
        public int SetID { get; set; }
        [Required]
        public string UserID { get; set; }
        public Set Set { get; set; }
        //public IdentityUser User { get; set; }
    }
}