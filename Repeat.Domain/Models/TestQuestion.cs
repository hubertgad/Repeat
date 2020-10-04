using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class TestQuestion
    {
        [Required]
        public int TestId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        public Test Test { get; set; }
        public Question Question { get; set; }
        public IList<ChoosenAnswer> ChoosenAnswers { get; set; }
    }
}