using Oid85.FinMarket.FollowTrend.Core.Requests;
using Oid85.FinMarket.FollowTrend.Core.Responses;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.Services
{
    public interface IMonitorService
    {
        Task<TerminalResponse> TerminalAsync(TerminalRequest request);
        Task<MonitorResponse> MonitorVersionAsync(MonitorRequest request);
        Task<EditPortfolioTotalSumResponse> EditPortfolioTotalSumAsync(EditPortfolioTotalSumRequest request);
    }
}
