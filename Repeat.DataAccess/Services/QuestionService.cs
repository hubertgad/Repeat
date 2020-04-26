using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Domain.Models;
using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    public class QuestionService
    {
        private readonly ApplicationDbContext _context;

        public QuestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync<T>(T t) where T : IEntity
        {
            await _context.AddAsync(t);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T t) where T : IEntity
        {
            _context.Update(t);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync<T>(T t) where T : IEntity
        {
            _context.Remove(t);
            await _context.SaveChangesAsync();
        }

        public bool ElementExists<T>(T t) where T : class, IEntity => _context.Set<T>().Any(e => e == t);

        public async Task<List<Question>> GetQuestionListAsync(string userID, int? categoryID, int? setID, bool shared = false)
        {
            var query = _context.Questions.Include(q => q.QuestionSets).Where(q => q.IsDeleted == false);

            if (setID != null && shared == true)
            {
                query = query.Where(q => q.QuestionSets.Any(w => w.SetID == setID));
            }
            else
            {
                query = query.Where(q => q.OwnerID == userID);
                if (categoryID == null && setID != null)
                {
                    query = query.Where(q => q.QuestionSets.Any(p => p.SetID == setID));
                }
                else if (categoryID != null)
                {
                    query = query.Where(q => q.CategoryID == categoryID);
                    if (setID != null)
                    {
                        query = query.Where(q => q.QuestionSets.Any(p => p.SetID == setID));
                    }
                }
            }

            var questions = await query.ToListAsync();

            foreach(var question in questions)
            {
                question.Answers =
                    await _context.Answers
                    .Where(q => q.QuestionID == question.ID && q.IsDeleted == false)
                    .ToListAsync();
            }

            return questions;
        }

        public async Task<List<Category>> GetCategoryListAsync(string userID)
            => await _context.Categories.Where(q => q.OwnerID == userID && q.IsDeleted == false).ToListAsync();

        public async Task<List<Set>> GetSetListAsync(string userID, bool includeShared = false)
        {
            var query = _context.Sets.Include(q => q.Shares)
                .Where(q => q.OwnerID == userID || q.Shares.Any(p => p.UserID == userID));

            if (includeShared == true)
            {
                query = query.Where(q => q.QuestionSets.Any())
                    .Include(q => q.QuestionSets).ThenInclude(q => q.Question).ThenInclude(q => q.Category);
            }
            if (includeShared == false)
            {
                query = query.Where(q => q.OwnerID == userID);
            }

            return await query.ToListAsync();
        }

        public async Task<List<IdentityUser>> GetUserListAsync(string userID)
            => await _context.Users.Where(q => q.Id != userID).ToListAsync();

        public async Task<Question> GetQuestionByIDAsync(int questionID, string userID)
        {
            var question = await _context.Questions
                .Where(m => m.OwnerID == userID && m.IsDeleted == false)
                .Include(o => o.Category)
                .Include(p => p.Picture)
                .Include(r => r.QuestionSets).ThenInclude(q => q.Set)
                .FirstOrDefaultAsync(m => m.ID == questionID);

            question.Answers = await _context.Answers
                .Where(q => q.QuestionID == questionID && q.IsDeleted == false).ToListAsync();

            return question;
        }

        public async Task<Category> GetCategoryByIDAsync(int categoryID, string userID)
        {
            return await _context.Categories
                .Where(m => m.OwnerID == userID && m.IsDeleted == false)
                .FirstOrDefaultAsync(m => m.ID == categoryID);
        }

        public async Task<Set> GetSetByIDAsync(int setID, string userID, bool includeShared = false)
        {
            var query = _context.Sets.Include(o => o.QuestionSets).ThenInclude(q => q.Question)
                .Where(m => m.ID == setID);

            if (includeShared == true)
            {
                return await query
                    .FirstOrDefaultAsync(q => q.OwnerID == userID || q.Shares.Any(p => p.UserID == userID));
            }
            else
            {
                return await query.FirstOrDefaultAsync(m => m.OwnerID == userID);
            }
        }

        public async Task<Test> GetTestByIDAsync(string userID, int? setID = null, int? testID = null)
        {
            var query = _context.Tests
                    .Include(q => q.TestQuestions).ThenInclude(q => q.Question).ThenInclude(q => q.Answers)
                    .Include(q => q.TestQuestions).ThenInclude(q => q.Question).ThenInclude(q => q.Picture)
                    .Include(q => q.QuestionResponses).ThenInclude(p => p.ChoosenAnswers);

            if (setID != null && testID == null)
            {
                return await query
                    .FirstOrDefaultAsync(q => q.SetID == setID && q.UserID == userID && q.IsCompleted == false);
            }
            else
            {
                return await query.Include(q => q.Set)
                    .FirstOrDefaultAsync(m => m.ID == testID && m.IsCompleted == true && m.UserID == userID);
            }
        }
    }
}