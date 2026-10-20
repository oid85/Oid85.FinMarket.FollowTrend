using Oid85.FinMarket.FollowTrend.Application.Helpers;
using Oid85.FinMarket.FollowTrend.Application.Models;
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
            Period = 10;
        }

        public override void SetSignalTickers() => SignalTickers = [.. FollowTrendHelper.GetUltimateSmootherSignalTickers(CandleData, CurrentDate.AddDays(-1), Period, CountTickers), MON];

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
    }
}
