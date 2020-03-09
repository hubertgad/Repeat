using System.Collections.Generic;

namespace Repeat.Models
{
    public class Test
    {
        public int ID { get; set; }
        public int SetID { get; set; }
        public Set Set { get; set; }
        public string UserID { get; set; }
        public bool IsCompleted { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public List<TestQuestion> TestQuestions { get; set; }
        public List<QuestionResponse> QuestionResponses { get; set; }

        public Test()
        {
        }

        public Test(Set set, string CUID, List<Question> questions)
        {
            this.SetID = set.ID;
            this.UserID = CUID;
            this.IsCompleted = false;
            this.CurrentQuestionIndex = 0;
            this.QuestionResponses = new List<QuestionResponse>();
            this.TestQuestions = new List<TestQuestion>();
            foreach (var question in questions)
            {
                this.TestQuestions.Add(new TestQuestion
                {
                    TestID = this.ID,
                    QuestionID = question.ID,
                    Index = questions.IndexOf(question)
                });
                QuestionResponses.Add(new QuestionResponse
                {
                    TestID = this.ID,
                    QuestionID = question.ID,
                    ChoosenAnswers = new List<ChoosenAnswer>()
                });
                foreach (var answer in question.Answers)
                {
                    this.QuestionResponses[questions.IndexOf(question)].ChoosenAnswers.Add(new ChoosenAnswer
                    {
                        QuestionResponseID = this.QuestionResponses[questions.IndexOf(question)].ID,
                        QuestionID = question.ID,
                        AnswerID = answer.ID,
                    }); ;
                }
            }
        }
    }
}
