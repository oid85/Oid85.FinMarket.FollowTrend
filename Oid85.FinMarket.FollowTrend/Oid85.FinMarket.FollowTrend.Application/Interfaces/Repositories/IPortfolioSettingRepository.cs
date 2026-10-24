using Oid85.FinMarket.FollowTrend.Core.Models;

namespace Oid85.FinMarket.FollowTrend.Application.Interfaces.Repositories
{
    public interface IPortfolioSettingRepository
    {
        Task<PortfolioSetting?> GetAsync(string name);
    }
}
