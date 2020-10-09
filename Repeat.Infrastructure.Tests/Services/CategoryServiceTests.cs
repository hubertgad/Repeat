using Microsoft.EntityFrameworkCore;
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

            _categoryService = new CategoryService(_serviceContext, _currentUserService);
        }

        [Test]
        public async Task AddCategoryAsync_WhenCalled_SaveCategoryInDbAsync()
        {
            var name = Guid.NewGuid().ToString();
            var category = new Category { Name = name };

            await _categoryService.AddCategoryAsync(category);

            var savedCategory = await _context.Categories.FirstAsync(q => q.Id == category.Id);
            Assert.That(savedCategory.Name, Is.EqualTo(name));
        }

        [Test]
        public async Task AddCategoryAsync_NotValidOwnerId_CorrectIdAsync()
        {
            var category = new Category { OwnerId = "not-valid-id" };

            await _categoryService.AddCategoryAsync(category);

            var savedCategory = await _context.Categories.FirstAsync(q => q.Id == category.Id);
            Assert.That(savedCategory.OwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task RemoveCategory_WhenCalled_RemoveCategoryFromDbAsync()
        {
            var category = await _setUpContext.Categories.FirstAsync();
            var initialCount = _setUpContext.Categories.Count();

            await _categoryService.RemoveCategoryAsync(category);

            Assert.That(_context.Categories.Count(), Is.LessThan(initialCount));
        }

        [Test]
        public async Task UpdateCategoryAsync_WhenCalled_UpdateCategoryInDbAsync()
        {
            var category = _setUpContext.Categories.FirstOrDefault();
            category.Name = "New name";

            await _categoryService.UpdateCategoryAsync(category);

            var savedCategory = await _context.Categories.FirstAsync(q => q.Id == category.Id);
            Assert.That(savedCategory.Name, Is.EqualTo("New name"));
        }

        [Test]
        public async Task UpdateCategoryAsync_NotValidOwnerId_CorrectIdAsync()
        {
            var category = _setUpContext.Categories.FirstOrDefault();
            category.Name = "New name";
            category.OwnerId = "not-valid-id";

            await _categoryService.UpdateCategoryAsync(category);

            var savedCategory = await _context.Categories.FirstAsync(q => q.Id == category.Id);
            Assert.That(savedCategory.OwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task GetCategoryById_CategoryExists_ReturnCategoryAsync()
        {
            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.That(result.Name, Is.EqualTo("Category 1"));
        }

        [Test]
        public async Task GetCategoryByIdAsync_CategoryExists_ContainListOfQuestionsAsync()
        {
            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.That(result.Questions, Is.Not.Null);
        }

        [Test]
        public async Task GetCategoryByIdAsync_CategoryDontExists_ReturnNullAsync()
        {
            var result = await _categoryService.GetCategoryByIdAsync(-2);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoriesForCurrentUser_WhenCalled_ReturnAllCategoriesAsync()
        {
            var result = await _categoryService.GetCategoriesForCurrentUserAsync();

            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}