using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastucture.Exceptions;
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
            model.OwnerId = _currentUserId;

            await _context.Sets.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public async Task AddShareAsync(int setId, string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(q => q.UserName == userName);

            if (user == null) return;

            var set = await _context.Sets.FindAsync(setId);

            if (set.OwnerId != _currentUserId) throw new AccessDeniedException();

            if (set.OwnerId == user.Id) return;

            if (_context.Shares.Any(q => q.SetId == setId && q.UserId == user.Id)) return;

            _context.Shares.Add(
                new Share
                {
                    SetId = setId,
                    UserId = user.Id
                });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSetAsync(Set model)
        {
            if (model.OwnerId != _currentUserId) throw new AccessDeniedException();

            var tests = _context.Tests.Where(q => q.SetId == model.Id);
            _context.Tests.RemoveRange(tests);

            _context.Sets.Remove(model);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionFromSetAsync(QuestionSet model)
        {
            if (model.Set.OwnerId != _currentUserId) throw new AccessDeniedException();

            _context.QuestionSets.Remove(model);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveShareAsync(Share model)
        {
            model.Set = _context.Sets.Find(model.SetId);
            if (model.Set.OwnerId != _currentUserId) throw new AccessDeniedException();

            _context.Shares.Remove(model);

            await _context.SaveChangesAsync();
        }

        public Task UpdateSetAsync(Set model)
        {
            if (model.OwnerId != _currentUserId) throw new AccessDeniedException();

            _context.Sets.Update(model);

            return _context.SaveChangesAsync();
        }

        public Task<Set> GetSetByIdAsync(int id)
        {
            return _context.Sets
                .Where(q => q.OwnerId == _currentUserId)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public Task<List<Set>> GetSetsForCurrentUserAsync()
        {
            return _context.Sets
                .Where(q => q.OwnerId == _currentUserId)
                .Include(q => q.Shares)
                    .ThenInclude(q => q.User)
                .ToListAsync();
        }
    }
}