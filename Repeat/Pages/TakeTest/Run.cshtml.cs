using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class RunModel : CustomPageModel
    {
        public RunModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        [BindProperty] public Test Test { get; set; }
        [BindProperty] public QuestionResponse QuestionResponse { get; set; }
        public Question CurrentQuestion { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            CurrentUserID = await GetUserIDAsync();

            var requestedSet = _context.Shares.FirstOrDefault(q => q.Set.ID == id && q.UserID == CurrentUserID);
            if (requestedSet == null)
            {
                return NotFound();
            }

            if (StartedTest(id) != null)
            {
                this.Test = StartedTest(id);
            }
            else
            {
                var questions = _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuestionSets.Any(q => q.SetID == id)).ToList();

                this.Test = new Test(_context.Sets.FirstOrDefault(q => q.ID == id), CurrentUserID, questions);
                _context.Add(this.Test);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }

            this.CurrentQuestion = GetCurrentQuestion();
            this.QuestionResponse = await GetQuestionResponseFromDBAsync();

            return Page();
        }

        private Question GetCurrentQuestion()
        {
            Question currentQuestion;
            try
            {
                currentQuestion = Test.Questions[Test.CurrentQuestionIndex];
            }
            catch
            {
                currentQuestion = Test.Questions[0];
            }
            return currentQuestion;
        }

        private async Task<QuestionResponse> GetQuestionResponseFromDBAsync()
        {
            return await _context
                .QuestionResponses.Where(q => q.TestID == this.Test.ID && q.QuestionID == this.CurrentQuestion.ID)
                .Include(q => q.ChoosenAnswers)
                .FirstOrDefaultAsync();
        }

        public async Task<IActionResult> OnPostPreviousAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Entry(Test).Property(q => q.CurrentQuestionIndex).IsModified = true;
            this.Test.CurrentQuestionIndex--;
            foreach (var choosenAnswer in QuestionResponse.ChoosenAnswers)
            {
                _context.Attach(choosenAnswer).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostNextAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Entry(Test).Property(q => q.CurrentQuestionIndex).IsModified = true;
            this.Test.CurrentQuestionIndex++;
            foreach (var choosenAnswer in QuestionResponse.ChoosenAnswers)
            {
                _context.Attach(choosenAnswer).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostFinishAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Entry(Test).Property(q => q.IsCompleted).IsModified = true;
            this.Test.IsCompleted = true;
            foreach (var choosenAnswer in QuestionResponse.ChoosenAnswers)
            {
                _context.Attach(choosenAnswer).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("./Details", new { id });
        }

        private Test StartedTest(int? id)
        {
            return _context.Tests
                .Include(q => q.Questions).ThenInclude(q => q.Answers)
                .Include(q => q.Questions).ThenInclude(q => q.Picture)
                .Where(q => q.SetID == id && q.UserID == CurrentUserID)
                .FirstOrDefault(o => o.IsCompleted == false);
        }
    }
}