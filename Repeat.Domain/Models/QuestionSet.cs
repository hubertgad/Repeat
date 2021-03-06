﻿using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class QuestionSet : IEntity
    {
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public int SetId { get; set; }
        public Question Question { get; set; }
        public Set Set { get; set; }
    }
}