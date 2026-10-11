using Oid85.FinMarket.FollowTrend.Core.Requests.ApiClient;
using Oid85.FinMarket.FollowTrend.Core.Responses.ApiClient;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.ApiClients
{
    /// <summary>
    /// Клиент сервиса FinMarket.Analytics
    /// </summary>
    public interface IAnalyticsApiClient
    {
        /// <summary>
        /// Получить рейтинг по фундаментальным данным
        /// </summary>
        Task<GetFundamentalRatingShortListResponse> GetFundamentalRatingShortListAsync(GetFundamentalRatingShortListRequest request);
    }
}
