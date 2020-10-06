using NUnit.Framework;
using Repeat.Domain.Interfaces;
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

            _testService = new TestService(_context, _currentUserService);
        }

        [Test]
        public async Task CreateTestFromSetAsync_WhenCalled_ShouldSaveTestInDbAsync()
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
        public async Task UpdateTestAsync_WhenCalled_ShouldUpdateTestInDbAsync()
        {
            var test = _setUpContext.Tests.Find(1);
            Assert.That(!test.IsCompleted);
            test.IsCompleted = true;

            await _testService.UpdateTestAsync(test);

            var testInDb = _context.Tests.Find(1);
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
    }
}