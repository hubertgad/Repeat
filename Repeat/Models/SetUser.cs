using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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