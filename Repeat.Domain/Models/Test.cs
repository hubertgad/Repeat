using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Test : Entity
    {
        [Required]
        public int SetID { get; set; }
        public Set Set { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public int CurrentQuestionID { get; set; }
        [Required]
        public IList<TestQuestion> TestQuestions { get; set; }
    }
}