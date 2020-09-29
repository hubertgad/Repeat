using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Question : Entity
    {
        [Required, MaxLength(5000), Display(Name = "Question Text")]
        public string QuestionText { get; set; }
        [Display(Name = "Category ID")]
        public int CategoryID { get; set; }
        [Required]
        public string OwnerID { get; set; }
        public ApplicationUser Owner { get; set; }
        [MaxLength(1000)]
        public string Reference { get; set; }
        public Picture Picture { get; set; }
        public Category Category { get; set; }
        public IList<Answer> Answers { get; set; }
        public virtual HashSet<QuestionSet> QuestionSets { get; set; }
    }
}