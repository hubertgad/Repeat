using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repeat.Models
{
    public class DeletedAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None), Key]
        public int ID { get; set; }
        [NotMapped]
        public int DeletedQuestionID { get; set; }
        [Required, MaxLength(1000)]
        public string AnswerText { get; set; }
        public bool IsTrue { get; set; }

        public DeletedAnswer()
        {
        }

        public DeletedAnswer(Answer answer)
        {
            this.ID = answer.ID;
            this.DeletedQuestionID = answer.QuestionID;
            this.AnswerText = answer.AnswerText;
            this.IsTrue = answer.IsTrue;
        }
    }
}