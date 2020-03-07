using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DeleteModel : CustomPageModel
    {
        public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) 
            : base(context, userManager)
        {
        }

        public Question Question { get; set; }
        public Category Category { get; set; }
        public List<Set> Sets { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.CurrentUserID = await GetUserIDAsync();

            this.Question = await GetQuestionFromDatabaseAsync(id);
            if (this.Question == null || this.Question.OwnerID != this.CurrentUserID)
            {
                return NotFound();
            }

            this.Sets = await GetSetsFromDatabase();

            return Page();
        }

        private async Task<Question> GetQuestionFromDatabaseAsync(int? id)
        {
            return await _context
                .Questions
                .Where(m => m.OwnerID == CurrentUserID)
                .Include(o => o.Category)
                .Include(n => n.Answers)
                .Include(p => p.Picture)
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        private async Task<List<Set>> GetSetsFromDatabase()
        {
            return await _context.Sets.Where(q => q.QuestionSets.Any(p => p.QuestionID == this.Question.ID)).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = _context.Questions.Find(id);
            if (question != null)
            {
                await CopyDataToBeDeletedAsync(question);
                _context.Questions.Remove(question);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    return NotFound();
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task CopyDataToBeDeletedAsync(Question question)
        {
            await _context.DeletedQuestions.AddAsync(new DeletedQuestion(question));
            if (question.Picture != null)
            {
                await _context.DeletedPictures.AddAsync(new DeletedPicture(_context.Pictures.FirstOrDefault(q => q.ID == question.ID)));
            }
            foreach (var answer in _context.Answers.Where(q => q.QuestionID == question.ID))
            {
                await _context.DeletedAnswers.AddAsync(new DeletedAnswer(answer));
            }
        }
    }
}