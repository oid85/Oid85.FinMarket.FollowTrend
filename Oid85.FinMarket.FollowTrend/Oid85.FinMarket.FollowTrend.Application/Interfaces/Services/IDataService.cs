using Oid85.FinMarket.FollowTrend.Core.Models;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.Services
{
    public interface IDataService
    {
        Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers);
        Task<Dictionary<string, List<Candle>>> GetWeekCandleDataAsync(List<string> tickers);
        Task<Dictionary<string, List<DateValue<double>>>> GetUltimateSmootherDataAsync(List<string> tickers, int period);
        Task<Dictionary<string, Instrument>> GetInstrumentDataAsync(List<string> tickers);
    }
}
