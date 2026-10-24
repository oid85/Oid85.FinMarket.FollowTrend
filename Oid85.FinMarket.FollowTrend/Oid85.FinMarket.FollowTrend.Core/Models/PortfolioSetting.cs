namespace Oid85.FinMarket.FollowTrend.Core.Models
{
    public class PortfolioSetting
    {
        public string Name { get; set; } = string.Empty;
        public string TickerListName { get; set; } = string.Empty;
        public List<string> Tickers { get; set; } = [];
        public double MoneySum { get; set; }
    }
}
