using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.FollowTrend.Common.KnownConstants;
using Oid85.FinMarket.FollowTrend.Infrastructure.Database.Entities;
using Oid85.FinMarket.FollowTrend.Infrastructure.Database.Schemas;

namespace Oid85.FinMarket.FollowTrend.Infrastructure.Database;

public class FollowTrendContext(DbContextOptions<FollowTrendContext> options) : DbContext(options)
{
    public DbSet<ParameterEntity> ParameterEntities { get; set; }
    public DbSet<StrategyExecuteResultEntity> StrategyExecuteResultEntities { get; set; }
    public DbSet<PortfolioSettingEntity> PortfolioSettingEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasDefaultSchema(KnownDatabaseSchemas.Default)
            .ApplyConfigurationsFromAssembly(
                typeof(FollowTrendContext).Assembly,
                type => type
                    .GetInterface(typeof(IFollowTrendSchema).ToString()) != null)
            .UseIdentityAlwaysColumns();
    }    
}