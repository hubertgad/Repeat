﻿using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    public class SetService : ISetService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _userId;

        public SetService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _userId = currentUserService.UserId;
        }

        public async Task AddSetAsync(Set model)
        {
            model.OwnerID = _userId;
            await _context.Sets.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public async Task AddShareAsync(int setId, string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(q => q.UserName == userName);

            if (user == null) return;

            var set = await _context.Sets.FindAsync(setId);

            if (set.OwnerID != _userId) return;

            if (set.OwnerID == user.Id) return;

            _context.Shares.Add(
                new Share
                {
                    SetID = setId,
                    UserID = user.Id
                });

            await _context.SaveChangesAsync();
        }

        public Task RemoveSetAsync(Set model)
        {
            var tests = _context.Tests.Where(q => q.SetID == model.ID);
            _context.Tests.RemoveRange(tests);

            _context.Sets.Remove(model);

            return _context.SaveChangesAsync();
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
            model.OwnerID = _userId;
            _context.Sets.Update(model);

            return _context.SaveChangesAsync();
        }

        public Task<Set> GetSetByIdAsync(int? id)
        {
            return _context.Sets
                .Where(q => q.OwnerID == _userId)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                .FirstOrDefaultAsync(q => q.ID == id);
        }

        public Task<List<Set>> GetSetsForCurrentUserAsync()
        {
            return _context.Sets
                .Where(q => q.OwnerID == _userId)
                .Include(q => q.Shares)
                    .ThenInclude(q => q.Set)
                .Include(q => q.Shares)
                    .ThenInclude(q => q.User)
                .ToListAsync();
        }
    }
}