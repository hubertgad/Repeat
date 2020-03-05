using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Models
{
    public class ChoosenAnswer
    {
        public int ID { get; set; }
        public int QuestionResponseID { get; set; }
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public bool GivenAnswer { get; set; }
//        public bool CorrectAnswer { get; set; }
    }
}
