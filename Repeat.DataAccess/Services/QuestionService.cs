using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _userId;

        public QuestionService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _userId = currentUserService.UserId;
        }

        public async Task AddQuestionAsync(Question model)
        {
            model.OwnerID = _userId;
            await _context.Questions.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question model, bool removePicture)
        {
            model.OwnerID = _userId;

            var currentQuestionSets = _context.QuestionSets.Where(q => q.QuestionID == model.ID).ToList();
            _context.QuestionSets.RemoveRange(currentQuestionSets.Except(model.QuestionSets));
            _context.QuestionSets.AddRange(model.QuestionSets.Except(currentQuestionSets));
            
            var answersToRemove = _context.Answers.Where(q => q.QuestionID == model.ID).ToList().Except(model.Answers);
            _context.Answers.RemoveRange(answersToRemove);

            if (removePicture == true)
            {
                _context.Pictures.Remove(model.Picture);
            }

            _context.Questions.Update(model);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionAsync(Question model)
        {
            _context.QuestionSets.RemoveRange(_context.QuestionSets.Where(q => q.QuestionID == model.ID));
            _context.TestQuestions.RemoveRange(_context.TestQuestions.Where(q => q.QuestionID == model.ID));
            _context.ChoosenAnswers.RemoveRange(_context.ChoosenAnswers.Where(q => q.QuestionID == model.ID));
            _context.Answers.RemoveRange(_context.Answers.Where(q => q.QuestionID == model.ID));
            _context.Questions.Remove(model);
            await _context.SaveChangesAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int? id)
        {
            Question question = await _context.Questions
                .Where(m => m.OwnerID == _userId)
                .Include(o => o.Category)
                .Include(p => p.Picture)
                .Include(r => r.QuestionSets).ThenInclude(q => q.Set)
                .FirstOrDefaultAsync(m => m.ID == id);

            question.Answers = await _context.Answers
                .Where(q => q.QuestionID == id).ToListAsync();

            return question;
        }

        public async Task<List<Question>> GetQuestionListAsync(int? catID, int? setID)
        {
            IQueryable<Question> query = _context.Questions
                .Include(q => q.QuestionSets)
                .Where(q => q.OwnerID == _userId);
            if (setID != null) query = query.Where(q => q.QuestionSets.Any(p => p.SetID == setID));
            if (catID != null) query = query.Where(q => q.CategoryID == catID);

            var questions = await query.ToListAsync();

            foreach (var question in questions)
            {
                question.Answers =
                    await _context.Answers
                    .Where(q => q.QuestionID == question.ID)
                    .ToListAsync();
            }

            return questions;
        }

        public Task<List<Set>> GetSetListAsync()
        {
            return _context.Sets
                .Include(q => q.Shares)
                .Where(q => q.OwnerID == _userId)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Category)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoryListAsync()
            => await _context.Categories.Where(q => q.OwnerID == _userId).ToListAsync();
    }
}