using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Services
{
    public class TestService : ITestService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _currentUserId;

        public TestService(IApplicationDbContext context, ICurrentUserService user)
        {
            _context = context;
            _currentUserId = user.UserId;
        }

        public async Task AddTestAsync(Test model)
        {
            _context.Tests.Add(model);
            await _context.SaveChangesAsync();

            foreach (var testQuestion in model.TestQuestions)
            {
                foreach (var choosenAnswer in testQuestion.ChoosenAnswers)
                {
                    choosenAnswer.TestId = model.Id;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateTestFromSetAsync(int? setId)
        {
            if (setId == null) return;

            var test = new Test
            {
                SetId = (int)setId,
                Set = await _context.Sets.FirstOrDefaultAsync(q => q.Id == setId),
                UserId = _currentUserId,
                IsCompleted = false,
                TestQuestions = new List<TestQuestion>()
            };

            var questions = await _context.Questions
                .Where(q => q.QuestionSets.Any(w => w.SetId == (int)setId))
                .Include(q => q.Answers)
                .ToListAsync();

            foreach (var question in questions)
            {
                int i = questions.IndexOf(question);
                if (i == 0) test.CurrentQuestionId = question.Id;

                test.TestQuestions.Add(
                    new TestQuestion
                    {
                        TestId = test.Id,
                        QuestionId = question.Id,
                        Question = question,
                        ChoosenAnswers = new List<ChoosenAnswer>()
                    });

                foreach (var answer in question.Answers)
                {
                    test.TestQuestions[i].ChoosenAnswers.Add(
                        new ChoosenAnswer
                        {
                            TestId = test.Id,
                            QuestionId = question.Id,
                            AnswerId = answer.Id,
                        });
                }
            }

            await AddTestAsync(test);
        }

        public Task UpdateTestAsync(Test model)
        {
            _context.Tests.Update(model);
            return _context.SaveChangesAsync();
        }

        public async Task UpdateChoosenAnswersAsync(IList<ChoosenAnswer> choosenAnswers)
        {
            var currentAnswers = await _context.ChoosenAnswers
                .Where(q => q.TestId == choosenAnswers.FirstOrDefault().TestId
                    && q.QuestionId == choosenAnswers.FirstOrDefault().QuestionId)
                .ToListAsync();

            for (int i = 0; i < currentAnswers.Count; i++)
            {
                currentAnswers[i].GivenAnswer = choosenAnswers[i].GivenAnswer;
            }

            await _context.SaveChangesAsync();
        }

        public async Task MoveToPreviousQuestion(int? setId)
        {
            var test = _context.Tests
                .FirstOrDefault(q => q.SetId == setId && !q.IsCompleted && q.UserId == _currentUserId);

            var currentQuestionIndex = test.TestQuestions.IndexOf(
                test.TestQuestions.FirstOrDefault(q => q.QuestionId == test.CurrentQuestionId));
            test.CurrentQuestionId = test.TestQuestions[currentQuestionIndex - 1].QuestionId;

            await _context.SaveChangesAsync();
        }

        public async Task MoveToNextQuestion(int? setId)
        {
            var test = _context.Tests
                .FirstOrDefault(q => q.SetId == setId && !q.IsCompleted && q.UserId == _currentUserId);

            var currentQuestionIndex = test.TestQuestions.IndexOf(
                test.TestQuestions.FirstOrDefault(q => q.QuestionId == test.CurrentQuestionId));
            test.CurrentQuestionId = test.TestQuestions[currentQuestionIndex + 1].QuestionId;

            await _context.SaveChangesAsync();
        }

        public async Task FinishTest(int? setId)
        {
            var test = _context.Tests
                .FirstOrDefault(q => q.SetId == setId && !q.IsCompleted && q.UserId == _currentUserId);

            test.IsCompleted = true;

            await _context.SaveChangesAsync();
        }

        public Task<Test> GetOpenTestBySetIdAsync(int? setId)
        {
            return _context.Tests
                .Where(q => q.UserId == _currentUserId && q.SetId == setId && !q.IsCompleted)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Answers)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Picture)
                .FirstOrDefaultAsync();
        }

        public async Task<Test> GetClosedTestBySetIdAsync(int? setId)
        {
            var tests = await _context.Tests
                .Where(q => q.UserId == _currentUserId && q.SetId == setId && q.IsCompleted)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Answers)
                .Include(q => q.TestQuestions)
                    .ThenInclude(p => p.ChoosenAnswers)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Picture)
                .Include(q => q.Set)
                .ToListAsync();

            return tests.LastOrDefault();
        }

        public Task<List<ChoosenAnswer>> GetChoosenAnswersAsync(int? testId, int? questionId)
        {
            return _context.ChoosenAnswers
                .Where(q => q.TestId == testId && q.QuestionId == questionId)
                .ToListAsync();
        }

        public Task<List<Set>> GetAvailableSetsAsync()
        {
            return _context.Sets
                .Where(q => q.OwnerId == _currentUserId || q.Shares.Any(p => p.UserId == _currentUserId))
                .Where(q => q.QuestionSets.Any())
                .Include(q => q.Shares)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Category)
                .Include(q => q.Owner)
                .ToListAsync();
        }
    }
}