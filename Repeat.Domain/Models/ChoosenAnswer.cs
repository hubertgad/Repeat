using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class ChoosenAnswer : Entity
    {
        [Required]
        public int TestId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public int AnswerId { get; set; }
        public bool GivenAnswer { get; set; }
        public Question Question { get; set; }
        public TestQuestion TestQuestion { get; set; }
    }
}