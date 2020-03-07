using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class ChoosenAnswer
    {
        [Key, Required]
        public int ID { get; set; }
        public int QuestionResponseID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public bool GivenAnswer { get; set; }
//        public bool CorrenctAnswer { get; set; }
    }
}