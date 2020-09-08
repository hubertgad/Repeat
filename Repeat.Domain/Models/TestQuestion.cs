namespace Repeat.Domain.Models
{
    public class TestQuestion
    {
        public int TestID { get; set; }
        public int QuestionID { get; set; }
        public Test Test { get; set; }
        public Question Question { get; set; }
        public int Index { get; set; }
    }
}