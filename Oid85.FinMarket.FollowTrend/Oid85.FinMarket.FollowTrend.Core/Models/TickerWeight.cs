using System.Diagnostics;
using Oid85.FinMarket.FollowTrend.Common.Extensions;

namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    [DebuggerDisplay("Ticker: {Ticker}, Weight: {Weight}")]
    public class TickerWeight
    {
        public string Ticker { get; set; } = string.Empty;
        public double Weight { get; set; } = 0.0;
    }
}
