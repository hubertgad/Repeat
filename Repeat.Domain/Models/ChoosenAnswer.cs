using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class ChoosenAnswer : Entity
    {
        [Required]
        public int TestID { get; set; }
        [Required]
        public int QuestionID { get; set; }
        [Required]
        public int AnswerID { get; set; }
        public bool GivenAnswer { get; set; }
        public TestQuestion TestQuestion { get; set; }
    }
}