using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ITestService _testService;

        public DetailsModel(ITestService testService)
        {
            _testService = testService;
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

            this.Test = await _testService.GetLastFinishedTestBySetIdAsync(id);

            if (this.Test == null)
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
            for (int i = 0; i < this.Test.TestQuestions.Count; i++)
            {
                points.Add(0);
                foreach (var choosenAnswer in this.Test.TestQuestions[i].ChoosenAnswers)
                {
                    if (choosenAnswer.GivenAnswer)
                    {
                        var answer =
                            this.Questions
                            .FirstOrDefault(q => q.Id == choosenAnswer.QuestionId)
                            .Answers
                            .FirstOrDefault(q => q.Id == choosenAnswer.AnswerId);

                        if (answer.IsTrue)
                        {
                            points[i]++;
                        }
                        else if (!answer.IsTrue)
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
                    if (answer.IsTrue)
                    {
                        maxPoints++;
                    }
                }
            }
            return maxPoints;
        }
    }
}