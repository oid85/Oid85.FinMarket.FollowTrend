using Oid85.FinMarket.FollowTrend.Core.Requests.ApiClient;
using Oid85.FinMarket.FollowTrend.Core.Responses.ApiClient;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.ApiClients
{
    /// <summary>
    /// Клиент сервиса FinMarket.Storage
    /// </summary>
    public interface IStorageApiClient
    {
        /// <summary>
        /// Получить свечи
        /// </summary>
        Task<GetCandleListResponse> GetCandleListAsync(GetCandleListRequest request);

        /// <summary>
        /// Получить инструменты
        /// </summary>
        Task<GetInstrumentListResponse> GetInstrumentListAsync(GetInstrumentListRequest request);
    }
}
