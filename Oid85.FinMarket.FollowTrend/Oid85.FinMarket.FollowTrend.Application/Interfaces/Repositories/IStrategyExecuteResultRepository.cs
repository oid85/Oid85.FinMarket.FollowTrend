using Oid85.FinMarket.FollowTrend.Core.Models;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.Repositories
{
    public interface IStrategyExecuteResultRepository
    {
        Task AddAsync(List<StrategyExecuteResult> strategyExecuteResults);
        Task<List<StrategyExecuteResult>> GetFilteredAsync();
        Task<List<StrategyExecuteResult>> GetAsync();
        Task DeleteAsync();
    }
}
