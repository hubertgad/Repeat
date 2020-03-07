using System.Collections.Generic;

namespace Repeat.Models
{
    public class QuestionResponse
    {
        public int ID { get; set; }
        public int TestID { get; set; }
        public int QuestionID { get; set; }
        public List<ChoosenAnswer> ChoosenAnswers { get; set; }
    }
}
