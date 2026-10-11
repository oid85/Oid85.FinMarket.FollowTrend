using Oid85.FinMarket.FollowTrend.Core.Requests.ApiClient;
using Oid85.FinMarket.FollowTrend.Core.Responses.ApiClient;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.ApiClients
{
    /// <summary>
    /// Клиент сервиса FinMarket.TraderFinam
    /// </summary>
    public interface ITraderFinamApiClient
    {
        /// <summary>
        /// Получить информацию о портфеле
        /// </summary>
        Task<PortfolioInfoResponse> GetPortfolioInfoAsync(PortfolioInfoRequest request);

        /// <summary>
        /// Получить список заданий Outbox
        /// </summary>
        Task<OutboxTaskListResponse> GetOutboxTaskListAsync(OutboxTaskListRequest request);
    }
}
