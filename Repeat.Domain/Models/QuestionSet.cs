using Repeat.Domain.SeedWork;

namespace Repeat.Domain.Models
{
    public class QuestionSet : IEntity
    {
        public int QuestionID { get; set; }
        public int SetID { get; set; }
        public Question Question { get; set; }
        public Set Set { get; set; }
    }
}