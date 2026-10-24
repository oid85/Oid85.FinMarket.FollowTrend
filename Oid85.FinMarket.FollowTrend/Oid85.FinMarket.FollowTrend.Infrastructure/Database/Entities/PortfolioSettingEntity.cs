using Oid85.FinMarket.FollowTrend.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.FollowTrend.Infrastructure.Database.Entities
{
    /// <summary>
    /// Параметры портфеля
    /// </summary>
    public class PortfolioSettingEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string TickerListName { get; set; } = string.Empty;
        public string Tickers { get; set; } = string.Empty;
        public double MoneySum { get; set; }
    }
}
