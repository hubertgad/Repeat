using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Models
{
    public class Test
    {
        [Key, Required]
        public int ID { get; set; }
        public int SetID { get; set; }
        public Set Set { get; set; }
        public string UserID { get; set; }
        public bool IsCompleted { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public List<Question> Questions { get; set; }
        public List<QuestionResponse> QuestionResponses { get; set; }

        public Test()
        {
        }
        public Test(Set set, string CUID, ApplicationDbContext context)
        {
            this.SetID = set.ID;
            this.UserID = CUID;
            this.IsCompleted = false;
            this.CurrentQuestionIndex = 0;
            this.Questions = context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuestionSets.Any(q => q.SetID == set.ID)).ToList();

            //context.SaveChanges();
            //foreach (var question in this.Questions)
            //{
            //    this.TestQuestions.Add(new TestQuestion { QuestionID = question.ID, TestID = this.ID });
            //}
            //context.SaveChanges();
            //// #TODO: jeden savechanges na końcu (?)
            this.QuestionResponses = new List<QuestionResponse>();
            foreach (var question in Questions)
            {
                QuestionResponses.Add(new QuestionResponse
                {
                    TestID = this.ID,
                    QuestionID = question.ID,
                    ChoosenAnswers = new List<ChoosenAnswer>()
                });
                context.SaveChanges();
                foreach (var answer in question.Answers)
                {
                    this.QuestionResponses[Questions.IndexOf(question)].ChoosenAnswers.Add(new ChoosenAnswer
                    {
                        QuestionResponseID = this.QuestionResponses[Questions.IndexOf(question)].ID,
                        QuestionID = question.ID,
                        AnswerID = answer.ID,
                    }); ;
                }
            }
        }
    }
}