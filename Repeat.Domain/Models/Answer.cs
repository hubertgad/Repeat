using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class Answer
    {
<<<<<<< HEAD
        public int ID { get; set; }
        public int QuestionID { get; set; }
        [MaxLength(1000)]
        public string AnswerText { get; set; }
        public bool IsTrue { get; set; }
=======
        public int ID { get; set; }        
        public int QuestionID { get; set; }        
        [Required, MaxLength(1000)]
        public string AnswerText { get; set; }        
        [Required]
        public bool IsTrue { get; set; }        
        public virtual Question Question { get; set; }
>>>>>>> f80dc051329a8de97127786e3431dcc6ddd2e08f

        public Answer()
        {
        }

        public Answer(DeletedAnswer deletedAnswer)
        {
            this.ID = deletedAnswer.ID;
            this.QuestionID = deletedAnswer.DeletedQuestionID;
            this.AnswerText = deletedAnswer.AnswerText;
            this.IsTrue = deletedAnswer.IsTrue;
        }
    }
}
