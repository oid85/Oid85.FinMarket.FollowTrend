using System.Diagnostics;

namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    [DebuggerDisplay("Date: {Date}, Ticker: {Ticker}, Weight: {Weight}")]
    public class TickerWeight
    {
        public DateOnly Date { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public double Weight { get; set; } = 0.0;
    }
}
