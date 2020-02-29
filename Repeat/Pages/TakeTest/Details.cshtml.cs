using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using Repeat.Models;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class DetailsModel : CustomPageModel
    {
        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public Test Test { get; set; }
        public List<Question> Questions { get; set; }
        public List<int> QuestionPoints { get; set; }
        public int TotalCollectedPoints { get; set; }
        public int AvailablePoints { get; set; }
        public double Result { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.CurrentUserID = await GetUserIDAsync();
            this.Test = await GetTestFromDatabaseAsync(id);
            
            if (Test == null || !RequestIsValid())
            {
                return NotFound();
            }

            this.Questions = await GetQuestionsFromDatabaseAsync();
            this.QuestionPoints = CalculatePoints();
            this.TotalCollectedPoints = this.QuestionPoints.Sum();
            this.AvailablePoints = CalculateAvailablePoints();
            this.Result = Math.Round((double)this.TotalCollectedPoints / this.AvailablePoints * 100, 2);

            return Page();
        }

        private async Task<List<Question>> GetQuestionsFromDatabaseAsync()
        {
            var questions = new List<Question>();
            foreach (var questionResponse in this.Test.QuestionResponses)
            {
                var question = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.ID == questionResponse.QuestionID);
                if (question == null)
                {
                    question = new Question(await _context.DeletedQuestions.FirstOrDefaultAsync(q => q.ID == questionResponse.QuestionID));
                    if (question != null)
                    {
                        question.Answers = new List<Answer>();
                        foreach (var answer in questionResponse.ChoosenAnswers)
                        {
                            var deletedAnswer = _context.DeletedAnswers.FirstOrDefault(q => q.ID == answer.AnswerID);
                            if (deletedAnswer != null)
                            {
                                question.Answers.Add(new Answer(deletedAnswer));
                            }
                        }
                    }
                }
                questions.Add(question);
            }
            return questions;
        }

        private async Task<Test> GetTestFromDatabaseAsync(int? id)
        {
            return await _context
                .Tests
                .Include(q => q.Questions).ThenInclude(q => q.Answers)
                .Include(q => q.Questions).ThenInclude(q => q.Picture)
                .Include(q => q.QuestionResponses).ThenInclude(q => q.ChoosenAnswers)
                .Include(q => q.Set)
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        private List<int> CalculatePoints()
        {
            var points = new List<int>();
            for (int i = 0; i < this.Test.QuestionResponses.Count; i++)
            {
                points.Add(0);
                foreach (var choosenAnswer in this.Test.QuestionResponses[i].ChoosenAnswers)
                {
                    if (choosenAnswer.GivenAnswer == true)
                    {
                        var answer = 
                            this.Questions
                            .FirstOrDefault(q => q.ID == choosenAnswer.QuestionID)
                            .Answers
                            .FirstOrDefault(q => q.ID == choosenAnswer.AnswerID);
                        if (answer == null)
                        {
                            answer = new Answer(_context.DeletedAnswers.FirstOrDefault(q => q.ID == choosenAnswer.AnswerID));
                        }

                        if (answer.IsTrue == true)
                        {
                            points[i]++;
                        }
                        else if (answer.IsTrue == false)
                        {
                            points[i] = 0;
                            break;
                        }
                    }
                }
            }
            return points;
        }

        private int CalculateAvailablePoints()
        {
            var maxPoints = 0;
            foreach (var question in this.Questions)
            {
                foreach (var answer in question.Answers)
                {
                    if (answer.IsTrue == true)
                    {
                        maxPoints++;
                    }
                }
            }
            return maxPoints;
        }

        private bool RequestIsValid()
        {
            if (Test.UserID == this.CurrentUserID && Test.IsCompleted == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}