using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Answer : Entity
    {
        public int QuestionID { get; set; }
        [MaxLength(1000)]
        public string AnswerText { get; set; }
        public bool IsTrue { get; set; }
        public bool IsDeleted { get; set; }
    }
}