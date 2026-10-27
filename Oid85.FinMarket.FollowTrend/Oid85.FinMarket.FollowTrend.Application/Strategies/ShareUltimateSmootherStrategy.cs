using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Strategies
{
    public class ShareUltimateSmootherStrategy : Strategy
    {
        public override List<string> GetDescription()
        {
            return [
                "Версия ShareUltimateSmoother",
                $"Период: {Period} дней"
                ];
        }

        public override void InitMonitorParameters()
        {
            Period = 30;
        }

        public override void SetSignalTickers() => SignalTickers = [.. GetSignalTickers(CurrentDate.AddDays(-1)), MON];

        public override void Execute()
        {
            CountTickers = Convert.ToInt32(Math.Truncate(Tickers.Count * 0.618));

            foreach (var date in Dates)
            {
                CurrentDate = date;
                TradingProcessor.CurrentDate = date;

                UpdatePrices();
                UpdateCandles();

                ClearMessages();
                SetSignalTickers();             
                SetWeights();
                SyncPositions();

                UpdateEquitySeries();
                UpdateMoneySeries();
            }
        }

        public List<string> GetSignalTickers(DateOnly date)
        {
            if (UltimateSmootherData is null) return [];

            DateOnly from = date.AddDays(-10);
            DateOnly to = date;

            var tickers = UltimateSmootherData
                .Where(x => x.Key != MON)
                .ToDictionary(k => k.Key, v => GetSignalDelta([.. v.Value.Where(x => x.Date >= from && x.Date <= to)]))
                .Where(x => x.Value.Signal == true)
                .OrderByDescending(x => x.Value.Delta)
                .Take(CountTickers)
                .Select(x => x.Key)
                .ToList();

            return tickers ?? [];
        }

        public static (bool Signal, double Delta) GetSignalDelta(List<DateValue<double>> values)
        {
            bool signal = values.Count >= 5 &&
            values[^4].Value < values[^3].Value &&
            values[^3].Value < values[^2].Value &&
            values[^2].Value < values[^1].Value;

            double delta = !signal ? 0.0 : Math.Abs((values[^1].Value - values[^4].Value) / values[^4].Value * 100.0);

            return (signal, delta);
        }
    }
}
