using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class Question
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(1000)]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; }
        [Required]
        [Display(Name = "Category ID")]
        public int CategoryID { get; set; }
        [Required]
        public string OwnerID { get; set; }
        public Picture Picture { get; set; }
        public Category Category { get; set; }
        public List<Answer> Answers { get; set; }
        public virtual ICollection<QuestionSet> QuestionSets { get; set; }
    }
}