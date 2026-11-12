using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Common.KnownConstants;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Strategies
{
    public class FutureWeekUltimateSmootherStrategy : Strategy
    {
        public override List<string> GetDescription()
        {
            return [
                "FutureWeekUltimateSmoother",
                $"Период: {Period} недель"
            ];
        }

        public override void InitParameters()
        {
            Timeframe = KnownTimeframes.Week;
            Period = 10;
            Leverage = 4.0;
        }

        public override void SetSignalTickers() => SignalTickers = [.. GetSignalTickers(CurrentDate.AddDays(-7)), MON];

        public override void Execute()
        {
            foreach (var week in Weeks)
            {
                CurrentDate = week.StartDate;
                TradingProcessor.CurrentDate = week.StartDate;

                UpdateWeekPrices();
                UpdateWeekCandles();

                ClearMessages();
                SetSignalTickers();             
                SetWeights();
                SyncPositions();

                UpdateEquity();
                UpdateMoney();
            }
        }

        public List<string> GetSignalTickers(DateOnly date)
        {            
            if (WeekUltimateSmootherData is null) return [];

            DateOnly from = date.AddMonths(-2);
            DateOnly to = date;

            var tickers = WeekUltimateSmootherData
                .Where(x => x.Key != MON)
                .ToDictionary(
                    k => k.Key,
                    v => GetSignal([.. v.Value.Where(x => x.Date >= from && x.Date <= to)]))
                .Where(x => x.Value == true)
                .Select(x => x.Key)
                .ToList();

            return tickers ?? [];
        }

        public static bool GetSignal(List<DateValue<double>> values) => 
            values.Count >= 5 &&
            values[^2].Value < values[^1].Value &&
            values[^3].Value < values[^2].Value;
    }
}
