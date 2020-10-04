using NUnit.Framework;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Tests.Services
{
    [TestFixture]
    class CategoryServiceTests : RepeatTestBase
    {
        private ICategoryService _categoryService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _categoryService = new CategoryService(_context, _currentUserService);
        }

        [Test]
        public async Task AddCategoryAsync_WhenCalled_ShouldSaveCategoryInDb()
        {
            var name = Guid.NewGuid().ToString();
            var category = new Category { Name = name };

            await _categoryService.AddCategoryAsync(category);

            var savedCategory = _context.Categories.First(q => q.Id == category.Id);
            Assert.That(savedCategory, Is.EqualTo(category));
            Assert.That(savedCategory.Name, Is.EqualTo(name));
        }

        [Test]
        public async Task AddCategoryAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var category = new Category { OwnerId = "not-valid-id" };

            await _categoryService.AddCategoryAsync(category);

            var savedOwnerId = _context.Categories.First(q => q.Id == category.Id).OwnerId;
            Assert.That(savedOwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task RemoveCategory_WhenCalled_ShouldRemoveCategoryFromDb()
        {
            var category = _setUpContext.Categories.FirstOrDefault();
            var initialCount = _setUpContext.Categories.Count();

            await _categoryService.RemoveCategoryAsync(category);

            Assert.That(_context.Categories.Count(), Is.EqualTo(initialCount - 1));
        }

        [Test]
        public async Task UpdateCategoryAsync_WhenCalled_ShouldUpdateCategoryInDb()
        {
            var category = _setUpContext.Categories.FirstOrDefault();
            category.Name = "New name";

            await _categoryService.UpdateCategoryAsync(category);

            var savedName = _context.Categories.First(q => q.Id == category.Id).Name;
            Assert.That(savedName, Is.EqualTo("New name"));
        }

        [Test]
        public async Task UpdateCategoryAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var category = _setUpContext.Categories.FirstOrDefault();
            category.Name = "New name";
            category.OwnerId = "not-valid-id";

            await _categoryService.UpdateCategoryAsync(category);

            var savedOwnerId = _context.Categories.First(q => q.Id == category.Id).OwnerId;
            Assert.That(savedOwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task GetCategoryById_CategoryExists_ShouldReturnCategory()
        {
            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.That(result.Name, Is.EqualTo("Category 1"));
        }

        [Test]
        public async Task GetCategoryByIdAsync_CategoryExists_ShouldContainListOfQuestions()
        {
            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.That(result.Questions, Is.Not.Null);
        }

        [Test]
        public async Task GetCategoryByIdAsync_CategoryDontExists_ShouldReturnNull()
        {
            var result = await _categoryService.GetCategoryByIdAsync(-2);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoriesForCurrentUser_WhenCalled_ShouldReturnAllCategories()
        {
            var result = await _categoryService.GetCategoriesForCurrentUserAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}