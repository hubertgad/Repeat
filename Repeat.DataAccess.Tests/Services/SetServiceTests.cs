using NUnit.Framework;
using Repeat.DataAccess.Services;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Tests.Services
{
    [TestFixture]
    class SetServiceTests : RepeatTestBase
    {
        private ISetService _setService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _setService = new SetService(_context, _userService);
        }

        [Test]
        public async Task AddSetAsync_WhenCalled_ShouldSaveShareInDb()
        {
            var set = new Set { ID = 3 };

            await _setService.AddSetAsync(set);

            var savedSet = _context.Sets.First(q => q.ID == set.ID);
            Assert.That(savedSet, Is.EqualTo(set));
        }

        [Test]
        public async Task AddSetAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var set = new Set { ID = 3, OwnerID = "SecondUser" };

            await _setService.AddSetAsync(set);

            var savedSet = _context.Sets.First(q => q.ID == set.ID);
            Assert.That(savedSet.OwnerID, Is.EqualTo(_userService.UserId));
        }
    }
}