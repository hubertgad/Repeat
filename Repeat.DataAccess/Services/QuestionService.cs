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

            foreach (var question in query)
            {
                question.Answers = 
                    await _context.Answers
                    .Where(q => q.QuestionID == question.ID && q.IsDeleted == false)
                    .ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<List<Category>> GetCategoryListAsync(string userID)
            => await _context.Categories
            .Where(q => q.OwnerID == userID && q.IsDeleted == false)
            .ToListAsync();

        public async Task<List<Set>> GetSetListAsync(string userID, bool includeShared = false)
        {
            if (includeShared == true)
            {
                return await _context.Sets
                    .Include(q => q.Shares)
                    .Include(q => q.QuestionSets).ThenInclude(q => q.Question).ThenInclude(q => q.Category)
                    .Where(q => q.OwnerID == userID || q.Shares.Any(p => p.UserID == userID))
                    .ToListAsync();
            }
            else
            {
                return await _context.Sets
                    .Include(q => q.Shares)
                    .Where(q => q.OwnerID == userID)
                    .ToListAsync();
            }
        }

        public async Task<List<QuestionSet>> GetQuestionSetListAsync(Question question)
            => await _context.QuestionSets.Where(q => q.QuestionID == question.ID).ToListAsync();

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
            question.Answers = _context.Answers
                .Where(q => q.QuestionID == questionID && q.IsDeleted == false).ToList();
            return question;
        }

        public async Task<Category> GetCategoryByIDAsync(int categoryID, string userID)
        {
            return 
                await _context.Categories
                .Where(m => m.OwnerID == userID && m.IsDeleted == false)
                .FirstOrDefaultAsync(m => m.ID == categoryID);
        }

        public async Task<Set> GetSetByIDAsync(int setID, string userID, bool includeShared = false)
        {
            if (includeShared == true)
            {
                return 
                    await _context.Sets
                    .Include(q => q.QuestionSets)
                    .FirstOrDefaultAsync(q => q.ID == setID &&
                        (q.OwnerID == userID || q.Shares.Any(p => p.UserID == userID)));
            }
            else
            {
                return 
                    await _context.Sets
                    .Where(m => m.OwnerID == userID)
                    .Include(o => o.QuestionSets).ThenInclude(q => q.Question)
                    .FirstOrDefaultAsync(m => m.ID == setID);
            }
        }

        public async Task<Test> GetTestByIDAsync(string userID, int? setID = null, int? testID = null)
        {
            if (setID != null && testID == null)
            {
                return await _context
                    .Tests
                    .Include(q => q.TestQuestions).ThenInclude(q => q.Question).ThenInclude(q => q.Answers)
                    .Include(q => q.TestQuestions).ThenInclude(q => q.Question).ThenInclude(q => q.Picture)
                    .Include(q => q.QuestionResponses).ThenInclude(p => p.ChoosenAnswers)
                    .FirstOrDefaultAsync(q => q.SetID == setID && q.UserID == userID && q.IsCompleted == false);
            }
            else
            {
                var test = await _context
                    .Tests
                    .Include(q => q.TestQuestions).ThenInclude(p => p.Question).ThenInclude(w => w.Answers)
                    .Include(q => q.TestQuestions).ThenInclude(p => p.Question).ThenInclude(w => w.Picture)
                    .Include(q => q.QuestionResponses).ThenInclude(q => q.ChoosenAnswers)
                    .Include(q => q.Set)
                    .FirstOrDefaultAsync(m => m.ID == testID && m.IsCompleted == true && m.UserID == userID);
                return test;
            }
        }

        public async Task<QuestionSet> GetQuestionSetByIDAsync(int questionID, int setID)
        {
            return 
                await _context.QuestionSets
                .FirstOrDefaultAsync(q => q.QuestionID == questionID && q.SetID == setID);
        }
    }
}