namespace Oid85.FinMarket.FollowTrend.Core.Configuration
{
    public class FollowTrendSettings
    {
        public StrategyExecuteResultFilterSettings StrategyExecuteResultFilter { get; set; } = new();
        public double StartMoneySum { get; set; }
        public List<string> DividendShareTickers { get; set; } = [];
        public List<string> EtfTickers { get; set; } = [];
        public List<string> FutureTickers { get; set; } = [];
    }

    public class StrategyExecuteResultFilterSettings
    {
        public double MinRecoveryFactor { get; set; }
        public double MinAnnualYieldReturn { get; set; }
        public double MaxDrawdownPercent { get; set; }
    }
}
