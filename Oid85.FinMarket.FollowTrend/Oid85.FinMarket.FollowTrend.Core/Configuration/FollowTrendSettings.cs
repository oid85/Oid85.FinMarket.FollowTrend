namespace Oid85.FinMarket.FollowTrend.Core.Configuration
{
    public class FollowTrendSettings
    {
        public StrategyExecuteResultFilterSettings StrategyExecuteResultFilter { get; set; } = new();
    }

    public class StrategyExecuteResultFilterSettings
    {
        public double MinRecoveryFactor { get; set; }
        public double MinAnnualYieldReturn { get; set; }
        public double MaxDrawdownPercent { get; set; }
    }
}
