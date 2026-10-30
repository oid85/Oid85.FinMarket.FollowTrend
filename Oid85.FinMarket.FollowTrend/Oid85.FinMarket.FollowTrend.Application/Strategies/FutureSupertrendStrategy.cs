using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Strategies
{
    public class FutureSupertrendStrategy : Strategy
    {
        public override List<string> GetDescription()
        {
            return [
                "FutureSupertrend",                
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
            if (CandleData is null) return [];            

            List<string> tickers = [];

            foreach (var (ticker, _) in CandleData.Where(x => x.Key != MON))
            {
                var candle = CandleData[ticker].FindLast(x => x.Date <= date);
                var supertrend = SupertrendLowerBandData[ticker].FindLast(x => x.Date <= date);

                if (candle is null) continue;
                if (supertrend is null) continue;

                if (supertrend.Value > candle.Close)
                    tickers.Add(ticker);
            }

            return tickers;
        }
    }
}
