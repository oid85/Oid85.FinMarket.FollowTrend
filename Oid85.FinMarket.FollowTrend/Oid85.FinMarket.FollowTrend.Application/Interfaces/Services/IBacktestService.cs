using Oid85.FinMarket.FollowTrend.Core.Requests;
using Oid85.FinMarket.FollowTrend.Core.Responses;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.Services
{
    public interface IBacktestService
    {
        Task<BacktestResponse> BacktestAsync(BacktestRequest request);
        Task<BacktestResultResponse> BacktestResultAsync(BacktestResultRequest request);
    }
}
