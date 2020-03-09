using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.DataAccess.Services;
using Repeat.Domain.Models;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class DetailsModel : CustomPageModel
    {
        public DetailsModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
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

            this.Test = await _qService.GetTestByIDAsync(this.CurrentUserID, null, (int)id);
            
            if (this.Test == null || !RequestIsValid())
            {
                return NotFound();
            }

            this.Questions = Test.TestQuestions.Select(q => q.Question).ToList();
            this.QuestionPoints = CalculatePoints();
            this.TotalCollectedPoints = this.QuestionPoints.Sum();
            this.AvailablePoints = CalculateAvailablePoints();
            this.Result = Math.Round((double)this.TotalCollectedPoints / this.AvailablePoints * 100, 2);

            return Page();
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