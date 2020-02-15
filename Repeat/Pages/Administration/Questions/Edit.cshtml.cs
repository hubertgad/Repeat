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
        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public Question Question { get; set; }
        [BindProperty]
        public FileUpload FileUpload { get; set; }
        [BindProperty]
        public List<int> SelectedSets { get; set; }
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.CurrentUserID = await GetUserIDAsync();
            Question = await _context.Questions
                .Include(o => o.Answers)
                .Include(o => o.QuestionSets)
                .Include(p => p.Picture)
                .Where(o => o.OwnerID == CurrentUserID)
                .FirstOrDefaultAsync(m => m.ID == id);
            BindDataToView();
            if (Question == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
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
            _context.QuestionSets.RemoveRange(_context.QuestionSets.Where(o => o.QuestionID == this.Question.ID));
            foreach (var item in SelectedSets)
            {
                _context.QuestionSets.Add(new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = item
                });
            }
            _context.Answers.RemoveRange(_context.Answers.Where(o => o.QuestionID == this.Question.ID));
            _context.Attach(Question).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(Question.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToPage($"./Index");
            //return RedirectToPage($"./Details?id={this.Question.ID}");
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.ID == id);
        }
        private void BindDataToView()
        {
            IEnumerable<int> selectedSetsValues = _context.Sets
                .Where(q => q.QuestionSets.Any(p => p.QuestionID == this.Question.ID))
                .Select(q => q.ID);
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name");
            ViewData["SetID"] = new MultiSelectList(_context.Sets, "ID", "Name", selectedSetsValues);
        }
        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}