using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Domain.Interfaces
{
    public interface ISetService
    {
        Task AddSetAsync(Set model);
        Task AddShareAsync(int setId, string userName);
        Task RemoveSetAsync(Set model);
        Task RemoveQuestionFromSetAsync(QuestionSet model);
        Task RemoveShareAsync(Share model);
        Task UpdateSetAsync(Set model);
        Task<Set> GetSetByIdAsync(int id);
        Task<List<Set>> GetSetsForCurrentUserAsync();
    }
}