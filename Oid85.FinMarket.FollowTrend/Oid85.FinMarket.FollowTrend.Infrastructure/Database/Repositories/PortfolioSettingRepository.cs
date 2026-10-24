using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Repositories;
using Oid85.FinMarket.FollowTrend.Core.Models;
using Oid85.FinMarket.FollowTrend.Infrastructure.Database.Entities;

namespace Oid85.FinMarket.FollowTrend.Infrastructure.Database.Repositories
{
    public class PortfolioSettingRepository(
        IDbContextFactory<FollowTrendContext> contextFactory)
        : IPortfolioSettingRepository
    {
        public async Task<PortfolioSetting?> GetAsync(string name)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.PortfolioSettingEntities.FirstOrDefaultAsync(x => x.Name == name);

            if (entity is null)
                return null;

            return Map(entity);
        }

        private static PortfolioSetting Map(PortfolioSettingEntity entity) => 
            new()
            {
                Name = entity.Name,
                TickerListName = entity.TickerListName,
                Tickers = JsonSerializer.Deserialize<List<string>>(entity.Tickers)!,
                MoneySum = entity.MoneySum
            };
    }
}
