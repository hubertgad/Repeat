using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string CurrentUserID { get; set; }
        [BindProperty] public Question Question { get; set; }
        [BindProperty] public List<Answer> Answers { get; set; }
        [BindProperty] public FileUpload FileUpload { get; set; }
        [BindProperty] public List<int> SelectedSets { get; set; }
        [BindProperty] public bool RemovePicture { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.CurrentUserID = await GetUserIDAsync();
            
            await GetQuestionFromDatabaseAsync(id);
            if (this.Question == null)
            {
                return NotFound();
            }

            this.Answers = this.Question.Answers;
            
            BindDataToView();
            return Page();
        }

        private async Task GetQuestionFromDatabaseAsync(int? id)
        {
            this.Question = await _context.Questions
                .Include(o => o.Answers)
                .Include(o => o.QuestionSets)
                .Include(p => p.Picture)
                .Where(o => o.OwnerID == this.CurrentUserID)
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();
            UpdateQuestionState();
            UpdateAnswersState();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(this.Question.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage($"./Details", new { id });
        }

        public async Task<IActionResult> OnPostRemoveAsync(int? id, int aid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();
            UpdateQuestionState();
            UpdateAnswersState();

            RemoveAnswer(aid);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists((int)aid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostAddAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();
            UpdateQuestionState();
            UpdateAnswersState();

            AddNewAnswer();

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

        private void BindDataToView()
        {
            IEnumerable<int> selectedSetsValues = _context.Sets
                .Where(q => q.QuestionSets.Any(p => p.QuestionID == this.Question.ID))
                .Select(q => q.ID);
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(q => q.OwnerID == this.CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new MultiSelectList(_context.Sets.Where(q => q.OwnerID == this.CurrentUserID), "ID", "Name", selectedSetsValues);
        }

        private void UpdateAnswersState()
        {
            foreach (var answer in this.Answers)
            {
                _context.Attach(answer).State = EntityState.Modified;
            }
        }

        private void UpdateQuestionSetsState()
        {
            _context.QuestionSets.RemoveRange(_context.QuestionSets.Where(o => o.QuestionID == this.Question.ID));
            foreach (var item in this.SelectedSets)
            {
                _context.QuestionSets.Add(new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = item
                });
            }
        }

        private async Task UpdatePictureStateAsync()
        {
            if (FileUpload.FormFile != null && FileUpload.FormFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await FileUpload.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < 2097152)
                {
                    Question.Picture.Data = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }
            else if (this.RemovePicture == true)
            {
                var picture = _context.Pictures.Find(this.Question.Picture.ID);
                _context.Pictures.Remove(picture);
                await _context.SaveChangesAsync();
            }
        }

        private void UpdateQuestionState() => _context.Attach(Question).State = EntityState.Modified;

        private void AddNewAnswer() => _context.Add(new Answer { QuestionID = this.Question.ID, AnswerText = "Type answer text..." });
        
        private void RemoveAnswer(int id)
        {
            var answer = _context.Answers.Find(id);
            _context.DeletedAnswers.Add(new DeletedAnswer(answer));
            _context.Remove(_context.Answers.Find(id));
        }

        private async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;

        private bool QuestionExists(int id) => _context.Questions.Any(e => e.ID == id);

        private bool AnswerExists(int id) => _context.Answers.Any(e => e.ID == id);
    }
}