using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Services
{
    public class SetService : ISetService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _currentUserId;

        public SetService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserId = currentUserService.UserId;
        }

        public async Task AddSetAsync(Set model)
        {
            model.OwnerID = _currentUserId;
            await _context.Sets.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public async Task AddShareAsync(int setId, string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(q => q.UserName == userName);

            if (user == null) return;

            var set = await _context.Sets.FindAsync(setId);

            if (set.OwnerID != _currentUserId) return;

            if (set.OwnerID == user.Id) return;

            _context.Shares.Add(
                new Share
                {
                    SetID = setId,
                    UserID = user.Id
                });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSetAsync(Set model)
        {
            if (model.OwnerID != _currentUserId) return;

            var tests = _context.Tests.Where(q => q.SetID == model.ID);
            _context.Tests.RemoveRange(tests);

            _context.Sets.Remove(model);

            await _context.SaveChangesAsync();
        }

        public Task RemoveQuestionFromSetAsync(QuestionSet model)
        {
            _context.QuestionSets.Remove(model);

            return _context.SaveChangesAsync();
        }

        public async Task RemoveShareAsync(Share model)
        {
            _context.Shares.Remove(model);

            await _context.SaveChangesAsync();
        }

        public Task UpdateSetAsync(Set model)
        {
            model.OwnerID = _currentUserId;
            _context.Sets.Update(model);

            return _context.SaveChangesAsync();
        }

        public Task<Set> GetSetByIdAsync(int? id)
        {
            return _context.Sets
                .Where(q => q.OwnerID == _currentUserId)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                .FirstOrDefaultAsync(q => q.ID == id);
        }

        public Task<List<Set>> GetSetsForCurrentUserAsync()
        {
            return _context.Sets
                .Where(q => q.OwnerID == _currentUserId)
                .Include(q => q.Shares)
                    .ThenInclude(q => q.Set)
                .Include(q => q.Shares)
                    .ThenInclude(q => q.User)
                .ToListAsync();
        }
    }
}