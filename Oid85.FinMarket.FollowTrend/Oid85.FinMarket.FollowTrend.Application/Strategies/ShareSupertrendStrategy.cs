using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Strategies
{
    public class ShareSupertrendStrategy : Strategy
    {
        public override List<string> GetDescription()
        {
            return [
                "ShareSupertrend",                
                $"Период: {Period} дней",
                $"Множитель: {Multiplier}"
            ];
        }

        public override void InitParameters()
        {
            Period = 30;
            Multiplier = 2.5;
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
            if (SupertrendLowerBandData is null) return [];

            List<string> tickers = [];

            foreach (var (ticker, _) in SupertrendLowerBandData.Where(x => x.Key != MON))
            {
                var supertrendLowerBand = SupertrendLowerBandData[ticker].FindLast(x => x.Date <= date);
                
                if (supertrendLowerBand is not null) 
                    tickers.Add(ticker);
            }

            return tickers;
        }
    }
}
