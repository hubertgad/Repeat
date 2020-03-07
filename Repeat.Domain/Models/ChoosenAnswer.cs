namespace Repeat.Models
{
    public class ChoosenAnswer
    {
        public int ID { get; set; }
        public int QuestionResponseID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public bool GivenAnswer { get; set; }
    }
}