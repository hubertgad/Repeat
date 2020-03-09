﻿using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Set : Entity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string OwnerID { get; set; }        
        public virtual List<QuestionSet> QuestionSets { get; set; }
        public virtual List<Share> Shares { get; set; }
    }
}