using Repeat.Domain.SeedWork;

namespace Repeat.Domain.Models
{
    public class ChoosenAnswer : Entity
    {
        public int QuestionResponseID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public bool GivenAnswer { get; set; }
    }
}