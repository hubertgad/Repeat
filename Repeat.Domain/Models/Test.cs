using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Test : Entity
    {
        [Required]
        public int? SetId { get; set; }
        public Set Set { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public int CurrentQuestionId { get; set; }
        [Required]
        public IList<TestQuestion> TestQuestions { get; set; }
    }
}