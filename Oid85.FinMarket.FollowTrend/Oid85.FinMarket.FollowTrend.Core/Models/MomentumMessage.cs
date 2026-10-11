namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    public class FollowTrendMessage
    {
        public DateOnly Date { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
    }
}
