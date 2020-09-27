using NUnit.Framework;
using Repeat.DataAccess.Services;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Tests.Services
{
    [TestFixture]
    class QuestionServiceTests : RepeatTestBase
    {
        private IQuestionService _questionService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Seed();

            _questionService = new QuestionService(_context, _userService);
        }

        [Test]
        public async Task AddQuestionAsync_WhenCalled_ShouldSaveQuestionInDb()
        {
            var question = new Question();

            await _questionService.AddQuestionAsync(question);

            var savedQuestion = _context.Questions.First(q => q == question);
            Assert.That(savedQuestion, Is.EqualTo(question));
        }

        [Test]
        public async Task AddQuestionAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var question = new Question { OwnerID = "not-valid-id" };

            await _questionService.AddQuestionAsync(question);

            var savedOwnerId = _context.Questions.First(q => q.ID == question.ID).OwnerID;
            Assert.That(savedOwnerId, Is.EqualTo(_userService.UserId));
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveQuestionFromDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            var initialCount = _setUpContext.Questions.Count();

            await _questionService.RemoveQuestionAsync(question);

            var currentCount = _context.Questions.Count();
            Assert.That(currentCount, Is.EqualTo(initialCount - 1));
            Assert.That(() => _context.Questions.First(q => q == question), Throws.Exception);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveAssociatedQuestionSetsFromDb()
        {
            var question = _setUpContext.Questions.First();

            await _questionService.RemoveQuestionAsync(question);

            var associatedQuestionSets = _context.QuestionSets
                .Where(q => q.QuestionID == question.ID)
                .ToList();
            Assert.That(associatedQuestionSets, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveAssociatedTestQuestionsFromDb()
        {
            var question = _setUpContext.Questions.First();

            await _questionService.RemoveQuestionAsync(question);

            var associatedTestQuestions = _context.TestQuestions
                .Where(q => q.QuestionID == question.ID)
                .ToList();
            Assert.That(associatedTestQuestions, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveAssociatedChoosenAnswersFromDb()
        {
            var question = _setUpContext.Questions.First();

            await _questionService.RemoveQuestionAsync(question);

            var associatedChoosenAnswers = _context.ChoosenAnswers
                .Where(q => q.QuestionID == question.ID)
                .ToList();
            Assert.That(associatedChoosenAnswers, Is.Empty);
        }

        /*
        [Test]
        public async Task UpdateQuestionAsync_WhenCalled_ShouldUpdateQuestionInDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            question.QuestionText = "New name";

            await _questionService.UpdateQuestionAsync(question, false);

            var savedName = _context.Questions.First(q => q.ID == question.ID).QuestionText;
            Assert.That(savedName, Is.EqualTo("New name"));
        }

        [Test]
        public async Task UpdateQuestionAsync_RemovePictureIsTrue_ShouldRemovePictureFromDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            Assert.That(question.Picture, Is.Not.Null);

            await _questionService.UpdateQuestionAsync(question, true);

            var savedQustion = _context.Questions.First(q => q == question);
            Assert.That(savedQustion.Picture, Is.Null);
        }

        [Test]
        public async Task UpdateQuestionAsync_RemovedAnswer_ShouldRemoveAnswerFromDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            var answersCount = question.Answers.Count;
            question.Answers.RemoveAt(0);

            await _questionService.UpdateQuestionAsync(question, false);

            var questionInDb = _context.Questions.First(q => q.ID == question.ID);
            Assert.That(question.Answers.Count, Is.EqualTo(answersCount - 1));
        }
        */

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldReturnQuestion()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question, Is.TypeOf<Question>());
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ShouldReturnFiveQuestions()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCategoryIdIsPassed_ShouldReturnOneQuestion()
        {
            var questions = await _questionService.GetQuestionListAsync(1, null);

            Assert.That(questions.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenSetIdIsPassed_ShouldReturnFiveQuestions()
        {
            var questions = await _questionService.GetQuestionListAsync(null, 1);

            Assert.That(questions.Count, Is.EqualTo(5));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ShouldReturnTwoSets()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetCategoryListAsync_WhenCalled_ShouldReturnFiveCategories()
        {
            var categories = await _questionService.GetCategoryListAsync();

            Assert.That(categories.Count, Is.EqualTo(5));
        }

        private void Seed()
        {
            var set = new Set { Name = "Sample set", OwnerID = _userService.UserId };
            var set2 = new Set { Name = "Sample set2", OwnerID = _userService.UserId };
            _setUpContext.Sets.Add(set);
            _setUpContext.Sets.Add(set2);

            var questions = new List<Question>();
            var testQuestions = new List<TestQuestion>();
            var choosenAnswers = new List<ChoosenAnswer>();

            for (var i = 0; i < 5; i++)
            {
                questions.Add(
                    new Question
                    {
                        QuestionText = $"Question { i + 1 }",
                        OwnerID = _userService.UserId,
                        Picture = new Picture { Data = new byte[] { 1, 3, 4, 5 } },
                        Category = new Category { Name = $"Category { i }", OwnerID = _userService.UserId },
                        Answers = new List<Answer>(),
                        QuestionSets = new HashSet<QuestionSet>()
                    });
                questions[i].Answers.Add(new Answer { Question = questions[i] });
                questions[i].Answers.Add(new Answer { Question = questions[i] });
                questions[i].QuestionSets.Add(
                    new QuestionSet 
                    {
                        Question = questions[i],
                        Set = set
                     });
                testQuestions.Add(
                    new TestQuestion
                    {
                        Question = questions[i],
                        Test = new Test()
                    });
                choosenAnswers.Add(
                    new ChoosenAnswer
                    {
                        Question = questions[i]
                    });
            }

            _setUpContext.Questions.AddRange(questions);
            _setUpContext.TestQuestions.AddRange(testQuestions);
            _setUpContext.ChoosenAnswers.AddRange(choosenAnswers);

            _setUpContext.SaveChanges();
        }
    }
}