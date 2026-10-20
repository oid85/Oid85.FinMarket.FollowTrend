using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Services;
using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Application.Services;
using Oid85.FinMarket.FollowTrend.Application.Strategies;

namespace Oid85.FinMarket.FollowTrend.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IFollowTrendService, FollowTrendService>();
        services.AddScoped<IBacktestService, BacktestService>();

        services.AddKeyedTransient<FollowTrendStrategy, FollowTrendUltimateSmootherStrategy>(nameof(FollowTrendUltimateSmootherStrategy));
    }
}