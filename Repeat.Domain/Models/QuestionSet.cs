namespace Repeat.Models
{
    public class QuestionSet
    {
        public int QuestionID { get; set; }
        public int SetID { get; set; }
        public Question Question { get; set; }
        public Set Set { get; set; }
    }
}