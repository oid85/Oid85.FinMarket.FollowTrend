using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Strategies
{
    public class FollowTrendUltimateSmootherStrategy : FollowTrendStrategy
    {
        public override List<string> GetDescription()
        {
            return [
                "Версия UltimateSmoother",
                $"Период: {Period} дней"
                ];
        }

        public override void InitMonitorParameters()
        {
            Period = 50;
        }

        public override void SetSignalTickers() => SignalTickers = [.. GetSignalTickers(CurrentDate.AddDays(-1)), MON];

        public override void Execute()
        {
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

            DateOnly from = date.AddDays(-5);
            DateOnly to = date;

            var tickers = UltimateSmootherData
                .Where(x => x.Key != MON)
                .ToDictionary(
                    k => k.Key,
                    v => GetSignal([.. v.Value.Where(x => x.Date >= from && x.Date <= to)]))
                .Where(x => x.Value == true)
                .Select(x => x.Key)
                .ToList();

            return tickers ?? [];
        }

        public static bool GetSignal(List<DateValue<double>> values)
        {
            if (values.Count < 5)
                return false;

            return 
                values[^3].Value < values[^2].Value &&
                values[^2].Value < values[^1].Value;
        }
    }
}
