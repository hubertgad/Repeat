using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Answer : Entity
    {
        public int QuestionID { get; set; }
        public Question Question { get; set; }
        [Required, MaxLength(1000)]
        public string AnswerText { get; set; }
        public bool IsTrue { get; set; }
    }
}