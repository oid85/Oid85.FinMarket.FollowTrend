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
        services.AddScoped<IMonitorService, MonitorService>();
        services.AddScoped<IBacktestService, BacktestService>();

        services.AddKeyedTransient<Strategy, ShareUltimateSmootherStrategy>(nameof(ShareUltimateSmootherStrategy));
        services.AddKeyedTransient<Strategy, EtfUltimateSmootherStrategy>(nameof(EtfUltimateSmootherStrategy));
        services.AddKeyedTransient<Strategy, FutureUltimateSmootherStrategy>(nameof(FutureUltimateSmootherStrategy));
        services.AddKeyedTransient<Strategy, ShareSupertrendStrategy>(nameof(ShareSupertrendStrategy));
		services.AddKeyedTransient<Strategy, EtfSupertrendStrategy>(nameof(EtfSupertrendStrategy));
		services.AddKeyedTransient<Strategy, FutureSupertrendStrategy>(nameof(FutureSupertrendStrategy));
    }
}