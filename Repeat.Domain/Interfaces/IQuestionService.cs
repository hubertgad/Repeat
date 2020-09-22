using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Domain.Interfaces
{
    public interface IQuestionService
    {
        Task AddQuestionAsync(Question model);
        Task UpdateQuestionAsync(Question model, bool removePicture);
        Task RemoveQuestionAsync(Question model);
        Task<Question> GetQuestionByIdAsync(int? id);
        Task<List<Question>> GetQuestionListAsync(int? catID, int? setID);
        Task<List<Set>> GetSetListAsync();
        Task<List<Category>> GetCategoryListAsync();

    }
}