using System.Diagnostics;

namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    [DebuggerDisplay("EntryPrice: {EntryPrice}, Price: {Price}, Profit: {Profit}, Size: {Size}, Cost: {Cost}")]
    public class Balance
    {
        public double? EntryPrice { get; set; } = null;
        public double Profit { get; set; } = 0.0;
        public double Price { get; set; } = 0.0;
        public double Size { get; set; } = 0.0;
        public double Cost => Price * Size;
    }
}
