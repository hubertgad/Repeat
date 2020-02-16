using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Repeat.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Question Question { get; set; }
        [BindProperty]
        public FileUpload FileUpload { get; set; }
        [BindProperty]
        public int[] SelectedSets { get; set; }
        [BindProperty]
        public int AnswersCount { get; set; }
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? answers)
        {
            CurrentUserID = await GetUserIDAsync();
            if (answers == null)
            {
                AnswersCount = 4;
            }
            else if (answers >= 10)
            {
                AnswersCount = 10;
            }
            else if (answers <= 2)
            {
                AnswersCount = 2;
            }
            else
            {
                AnswersCount = (int)answers;
            }
            
            BindDataToView();
            
            this.Question = new Question { Answers = new List<Answer>() };
            for (int i = 0; i < AnswersCount; i++)
            {
                this.Question.Answers.Add(new Answer());
            }
            this.Question.OwnerID = CurrentUserID;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            BindDataToView();
            
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
                    this.Question.Picture = new Picture();
                    this.Question.Picture.Data = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }

            _context.Questions.Add(this.Question);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            foreach (var item in SelectedSets)
            {
                _context.QuestionSets.Add(new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = item
                });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        public IActionResult OnPostMore() => RedirectToPage("./Create", new { answers = ++AnswersCount });
        public IActionResult OnPostLess() => RedirectToPage("./Create", new { answers = --AnswersCount });        

        private void BindDataToView()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(q => q.OwnerID == CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new SelectList(_context.Sets.Where(q => q.OwnerID == CurrentUserID), "ID", "Name");
        }

        private async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
    }
}