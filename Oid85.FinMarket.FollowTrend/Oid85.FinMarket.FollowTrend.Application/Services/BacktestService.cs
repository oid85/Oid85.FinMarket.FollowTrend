using Microsoft.Extensions.Options;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Repositories;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Services;
using Oid85.FinMarket.FollowTrend.Common.KnownConstants;
using Oid85.FinMarket.FollowTrend.Core.Configuration;
using Oid85.FinMarket.FollowTrend.Core.Models;
using Oid85.FinMarket.FollowTrend.Core.Requests;
using Oid85.FinMarket.FollowTrend.Core.Responses;


namespace Oid85.FinMarket.FollowTrend.Application.Services
{
    public class BacktestService(
        IOptions<FollowTrendSettings> options,
        IDataService dataService,
        IStrategyExecuteResultRepository strategyExecuteResultRepository,
        IPortfolioSettingRepository portfolioSettingRepository,
        IServiceProvider serviceProvider) 
        : IBacktestService
    {
        public async Task<BacktestResponse> BacktestAsync(BacktestRequest request)
        {            
            return new ();
        }

        public async Task<BacktestResultResponse> BacktestResultAsync(BacktestResultRequest request)
        {
            const int countLimit = 30;

            var strategyExecuteResults = await strategyExecuteResultRepository.GetFilteredAsync();

            var orderedBacktestResults = strategyExecuteResults
                    .Select(x =>
                    new BacktestResult
                    {
                        Id = x.Id,
                        StrategyName = x.StrategyName,
                        StrategyParams = x.StrategyParams,
                        RecoveryFactor = x.RecoveryFactor,
                        NetProfit = x.NetProfit,
                        AnnualYieldReturn = x.AnnualYieldReturn,
                    })
                    .OrderByDescending(x => x.AnnualYieldReturn)
                    .Take(countLimit)
                    .ToList();

            int number = 1;
            foreach (var orderedBacktestResult in orderedBacktestResults)
                orderedBacktestResult.Number = number++;

            return new BacktestResultResponse
            {
                BacktestResults = orderedBacktestResults,

                EquitySeries = [.. orderedBacktestResults
                    .Select(x =>
                    new DiagramSeries
                    {
                        Name = $"{x.Number}. {x.StrategyName}: {x.StrategyParams}",
                        Color = KnownColors.DarkBlue,
                        ColorFill = KnownColors.DarkBlue,
                        Data = strategyExecuteResults.Find(xx => xx.Id == x.Id)?.EquityCurve ?? []
                    })]
            };
        }
    }
}
