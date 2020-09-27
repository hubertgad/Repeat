using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repeat.DataAccess.Data;
using Repeat.Domain.Interfaces;
using System;

namespace Repeat.DataAccess.Services
{
    [SetUpFixture]
    public class RepeatTestBase
    {
        protected IApplicationDbContext _setUpContext;
        protected IApplicationDbContext _context;
        protected ICurrentUserService _userService;

        [SetUp]
        public virtual void SetUp()
        {
            _userService = new CurrentUserService { UserId = Guid.NewGuid().ToString() };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _setUpContext = new ApplicationDbContext(options);
            _context = new ApplicationDbContext(options);

            _setUpContext.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _setUpContext.Database.EnsureDeleted();

            _setUpContext.Dispose();
            _context.Dispose();
        }
    }
}