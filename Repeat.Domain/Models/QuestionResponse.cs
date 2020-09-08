using Repeat.Domain.SeedWork;
using System.Collections.Generic;

namespace Repeat.Domain.Models
{
    public class QuestionResponse : Entity
    {
        public int TestID { get; set; }
        public int QuestionID { get; set; }
        public List<ChoosenAnswer> ChoosenAnswers { get; set; }
    }
}