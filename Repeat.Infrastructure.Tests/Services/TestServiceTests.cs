using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastructure.Services;
using Repeat.Infrastucture.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Tests.Services
{
    [TestFixture]
    class TestServiceTests : RepeatTestBase
    {
        private ITestService _testService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _testService = new TestService(_serviceContext, _currentUserService);
        }

        [Test]
        public async Task CreateTestFromSetAsync_WhenCalled_SaveTestInDbAsync()
        {
            var initialTestCount = _setUpContext.Tests.Count();

            await _testService.CreateTestFromSetAsync(1);

            var currentTestCount = _context.Tests.Count();
            Assert.That(currentTestCount, Is.GreaterThan(initialTestCount));
        }

        [Test]
        public void CreateTestFromSetAsync_NotSharedToUser_ThrowAccessDeniedException()
        {
            Assert.That(async () => await _testService.CreateTestFromSetAsync(3),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task UpdateTestAsync_WhenCalled_UpdateTestInDbAsync()
        {
            var test = await _setUpContext.Tests.FindAsync(3);
            Assert.That(!test.IsCompleted);
            test.IsCompleted = true;

            await _testService.UpdateTestAsync(test);

            var testInDb = await _context.Tests.FindAsync(3);
            Assert.That(testInDb.IsCompleted);
        }

        [Test]
        public void UpdateTestAsync_UserIsNotOwner_ThrowAccessDeniedException()
        {
            var test = _setUpContext.Tests.Find(2);
            Assert.That(test.UserId != _currentUserService.UserId);
            test.IsCompleted = true;

            Assert.That(async () => await _testService.UpdateTestAsync(test),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task UpdateChoosenAnswersAsync_WhenCalled_UpdateChoosenAnswersInDbAsync()
        {
            var choosenAnwers = await _setUpContext.ChoosenAnswers.ToListAsync();
            Assert.That(choosenAnwers.FirstOrDefault().GivenAnswer, Is.False);
            choosenAnwers.FirstOrDefault().GivenAnswer = true;

            await _testService.UpdateChoosenAnswersAsync(choosenAnwers);

            var currentChoosenAnswers = await _context.ChoosenAnswers.ToListAsync();
            Assert.That(currentChoosenAnswers.FirstOrDefault().GivenAnswer, Is.True);
        }

        [Test]
        public void MoveToPreviousQuestionAsync_NotSharedToUser_ThrowAccessDeniedEception()
        {
            Assert.That(async () => await _testService.MoveToPreviousQuestionAsync(3),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task MoveToPreviousQuestionAsync_WhenCalled_ChangeCurrentQuestionIdToPreviousAsync()
        {
            var initialQuestionId = _setUpContext.Tests
                .LastOrDefault(q => q.SetId == 1 && q.UserId == _currentUserService.UserId && !q.IsCompleted)
                .CurrentQuestionId;

            await _testService.MoveToPreviousQuestionAsync(1);

            var currentQuestionId =  _context.Tests
                .LastOrDefault(q => q.SetId == 1 && q.UserId == _currentUserService.UserId && !q.IsCompleted)
                .CurrentQuestionId;
            Assert.That(currentQuestionId, Is.LessThan(initialQuestionId));
        }

        [Test]
        public void MoveToNextQuestionAsync_NotSharedToUser_ThrowAccessDeniedEception()
        {
            Assert.That(async () => await _testService.MoveToNextQuestionAsync(3),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task MoveToNextQuestionAsync_WhenCalled_ChangeCurrentQuestionIdToNextAsync()
        {
            var initialQuestionId = _setUpContext.Tests
                .LastOrDefault(q => q.SetId == 1 && q.UserId == _currentUserService.UserId && !q.IsCompleted)
                .CurrentQuestionId;

            await _testService.MoveToNextQuestionAsync(1);

            var currentQuestionId = _context.Tests
                .LastOrDefault(q => q.SetId == 1 && q.UserId == _currentUserService.UserId && !q.IsCompleted)
                .CurrentQuestionId;
            Assert.That(currentQuestionId, Is.GreaterThan(initialQuestionId));
        }

        [Test]
        public void FinishTestAsync_NotSharedToUser_ThrowAccessDeniedEception()
        {
            Assert.That(async () => await _testService.FinishTestAsync(3),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task FinishTestAsync_WhenCalled_ChangeIsCompletedToTrueInDbAsync()
        {
            var initial = _setUpContext.Tests
                .LastOrDefault(q => q.SetId == 1 && q.UserId == _currentUserService.UserId && !q.IsCompleted);
            Assert.That(initial.IsCompleted, Is.False);

            await _testService.FinishTestAsync(1);

            var current = _context.Tests
                .LastOrDefault(q => q.SetId == 1 && q.UserId == _currentUserService.UserId && q.IsCompleted);
            Assert.That(initial.Id, Is.EqualTo(current.Id));
            Assert.That(current.IsCompleted, Is.True);
        }

        [Test]
        public async Task GetOpenedTestByIdAsync_WhenCalled_GetListOfTestsAsync()
        {
            var test = await _testService.GetOpenTestBySetIdAsync(1);

            Assert.That(test, Is.TypeOf<Test>());
        }

        [Test]
        public async Task GetOpenedTestByIdAsync_WhenCalled_ContainQuestionsWithAnswersAsync()
        {
            var test = await _testService.GetOpenTestBySetIdAsync(1);

            Assert.That(test.TestQuestions.FirstOrDefault().Question.Answers, 
                Has.All.TypeOf<Answer>());
        }

        [Test]
        public async Task GetOpenedTestByIdAsync_WhenCalled_ContainQuestionsWithPictureAsync()
        {
            var test = await _testService.GetOpenTestBySetIdAsync(1);

            Assert.That(test.TestQuestions.FirstOrDefault().Question.Picture.Data, 
                Is.TypeOf<byte[]>());
        }

        [Test]
        public async Task GetLastFinishedTestBySetIdAsync_WhenCalled_ReturnTestAsync()
        {
            var test = await _testService.GetLastFinishedTestBySetIdAsync(1);

            Assert.That(test, Is.TypeOf<Test>());
        }

        [Test]
        public async Task GetLastFinishedTestBySetIdAsync_WhenCalled_ContainQuestionsAndAnswersAsync()
        {
            var test = await _testService.GetLastFinishedTestBySetIdAsync(1);

            Assert.That(test.TestQuestions.FirstOrDefault().Question.Answers,
                Has.All.TypeOf<Answer>());
        }

        [Test]
        public async Task GetLastFinishedTestBySetIdAsync_WhenCalled_ContainQuestionsWithPictureAsync()
        {
            var test = await _testService.GetLastFinishedTestBySetIdAsync(1);

            Assert.That(test.TestQuestions.FirstOrDefault().Question.Picture.Data,
                Is.TypeOf<byte[]>());
        }

        [Test]
        public async Task GetLastFinishedTestBySetIdAsync_WhenCalled_ContainChoosenAnswersAsync()
        {
            var test = await _testService.GetLastFinishedTestBySetIdAsync(1);

            Assert.That(test.TestQuestions.FirstOrDefault().ChoosenAnswers,
                Has.All.TypeOf<ChoosenAnswer>());
        }

        [Test]
        public async Task GetLastFinishedTestBySetIdAsync_WhenCalled_ContainSetAsync()
        {
            var test = await _testService.GetLastFinishedTestBySetIdAsync(1);

            Assert.That(test.Set, Is.TypeOf<Set>());
        }

        [Test]
        public async Task GetChoosenAnswersAsync_WhenCalled_ReturnListOfChoosenAnswers()
        {
            var choosenAnswers = await _testService.GetChoosenAnswersAsync(1, 1);

            Assert.That(choosenAnswers, Has.All.TypeOf<ChoosenAnswer>());
        }

        [Test]
        public async Task GetChoosenAnswersAsync_NotValidTestId_ReturnEmptyList()
        {
            var choosenAnswers = await _testService.GetChoosenAnswersAsync(-2, 1);

            Assert.That(choosenAnswers, Is.Empty);
        }

        [Test]
        public async Task GetChoosenAnswersAsync_NotValidQuestionId_ReturnEmptyList()
        {
            var choosenAnswers = await _testService.GetChoosenAnswersAsync(1, -2);

            Assert.That(choosenAnswers, Is.Empty);
        }

        [Test]
        public async Task GetAvailableSetsAsync_WhenCalled_ReturnListOfSets()
        {
            var sets = await _testService.GetAvailableSetsAsync();

            Assert.That(sets, Has.All.TypeOf<Set>());
        }

        [Test]
        public async Task GetAvailableSetsAsync_WhenCalled_ContainShares()
        {
            var sets = await _testService.GetAvailableSetsAsync();

            Assert.That(sets.FirstOrDefault().Shares, Has.All.TypeOf<Share>());
        }

        [Test]
        public async Task GetAvailableSetsAsync_WhenCalled_ContainQuestionsWithCategories()
        {
            var sets = await _testService.GetAvailableSetsAsync();

            Assert.That(sets.FirstOrDefault().QuestionSets.FirstOrDefault().Question.Category, 
                Is.TypeOf<Category>());
        }

        [Test]
        public async Task GetAvailableSetsAsync_WhenCalled_ContainOwner()
        {
            var sets = await _testService.GetAvailableSetsAsync();

            Assert.That(sets.FirstOrDefault().Owner, Is.TypeOf<ApplicationUser>());
        }
    }
}