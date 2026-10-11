namespace Oid85.FinMarket.FollowTrend.Common.KnownConstants;

public static class KnownSettingsKeys
{
    public const string PostgresFollowTrendConnectionString = "Postgres:FollowTrendConnectionString";
    public const string PostgresApplyMigrationsOnStart = "Postgres:ApplyMigrationsOnStart";
    public const string DeployPort = "DeployPort";
    public const string FinMarketStorageServiceApiClientBaseAddress = "FinMarketStorageServiceApiClient:BaseAddress";
    public const string FinMarketAnalyticsServiceApiClientBaseAddress = "FinMarketAnalyticsServiceApiClient:BaseAddress";
    public const string FinMarketTraderFinamServiceApiClientBaseAddress = "FinMarketTraderFinamServiceApiClient:BaseAddress";
}