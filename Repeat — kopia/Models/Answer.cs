using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Repeat.Models
{
    public class Answer
    {
        public int ID { get; set; }
        [Required]
        public int QuestionID { get; set; }
        [Required]
        [MaxLength(1000)]
        public string AnswerText { get; set; }
        [Required]
        public bool IsTrue { get; set; }
        public Question Question { get; set; }
    }
}