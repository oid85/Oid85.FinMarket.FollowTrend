using System.Diagnostics;
using Oid85.FinMarket.FollowTrend.Common.Extensions;

namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    [DebuggerDisplay("Ticker: {Ticker}, Weight: {Weight}")]
    public class PositionData
    {
        public string Ticker { get; set; } = string.Empty;
        public Candle Candle { get; set; } = new Candle();
        public int Lot { get; set; } = 1;
        public double Weight { get; set; } = 0.0;
        public double EntryPrice { get; set; } = 0.0;
        public double ProfitPercent => Candle.Close == 0.0 || EntryPrice == 0.0 ? 0.0 : ((Candle.Close - EntryPrice) / Candle.Close * 100.0).RoundTo(2);
    }
}
