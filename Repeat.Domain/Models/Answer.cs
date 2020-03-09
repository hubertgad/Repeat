using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class Answer
    {
        public int ID { get; set; }
        public int QuestionID { get; set; }
        [MaxLength(1000)]
        public string AnswerText { get; set; }
        public bool IsTrue { get; set; }
        public bool IsDeleted { get; set; }
    }
}
