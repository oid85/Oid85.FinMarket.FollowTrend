using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Common.KnownConstants;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Strategies
{
    public class EtfWeekSupertrendStrategy : Strategy
    {
        public override List<string> GetDescription()
        {
            return [
                "EtfWeekSupertrend",
                $"Период: {Period} дней",
                $"Множитель: {Multiplier}"
            ];
        }

        public override void InitParameters()
        {
            Timeframe = KnownTimeframes.Week;
            Period = 10;
            Multiplier = 3.0;
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

                UpdateEquitySeries();
                UpdateMoneySeries();
            }
        }

        public List<string> GetSignalTickers(DateOnly date)
        {
            if (WeekSupertrendLowerBandData is null) return [];

            List<string> tickers = [];

            foreach (var (ticker, _) in WeekSupertrendLowerBandData.Where(x => x.Key != MON))
            {
                var supertrendLowerBand = WeekSupertrendLowerBandData[ticker].FindLast(x => x.Date <= date);

                if (supertrendLowerBand is not null)
                    tickers.Add(ticker);
            }

            return tickers;
        }
    }
}
