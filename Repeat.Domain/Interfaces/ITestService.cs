using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Domain.Interfaces
{
    public interface ITestService
    {
        Task CreateTestFromSetAsync(int setId);
        Task UpdateTestAsync(Test model);
        Task UpdateChoosenAnswersAsync(IList<ChoosenAnswer> choosenAnswers);
        Task MoveToPreviousQuestionAsync(int setId);
        Task MoveToNextQuestionAsync(int setId);
        Task FinishTestAsync(int setId);
        Task<Test> GetOpenTestBySetIdAsync(int setId);
        Task<Test> GetLastFinishedTestBySetIdAsync(int? setId);
        Task<List<ChoosenAnswer>> GetChoosenAnswersAsync(int? testId, int? questionId);
        Task<List<Set>> GetAvailableSetsAsync();
    }
}