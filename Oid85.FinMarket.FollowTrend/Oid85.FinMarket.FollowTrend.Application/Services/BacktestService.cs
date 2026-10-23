using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Repositories;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Services;
using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Application.Strategies;
using Oid85.FinMarket.FollowTrend.Common.KnownConstants;
using Oid85.FinMarket.FollowTrend.Core.Configuration;
using Oid85.FinMarket.FollowTrend.Core.Models;
using Oid85.FinMarket.FollowTrend.Core.Requests;
using Oid85.FinMarket.FollowTrend.Core.Responses;
using static Oid85.FinMarket.FollowTrend.Application.Mapping.ApplicationMapper;


namespace Oid85.FinMarket.FollowTrend.Application.Services
{
    public class BacktestService(
        IOptions<FollowTrendSettings> options,
        IDataService dataService,
        IStrategyExecuteResultRepository strategyExecuteResultRepository,
        IServiceProvider serviceProvider) 
        : IBacktestService
    {
        private readonly List<int> PARAMETER_LIST_PERIOD = [10, 25, 50]; 

        private readonly DateOnly _from = new DateOnly(2021, 1, 1);
        private readonly DateOnly _to = DateOnly.FromDateTime(DateTime.Today);
        private List<string> _tickers = [];
        private Dictionary<string, List<Candle>> _candleData = [];
        private Dictionary<string, Instrument> _instrumentData = [];

        public async Task<BacktestResponse> BacktestAsync(BacktestRequest request)
        {
            var followTrendSettings = options.Value;

            _tickers = followTrendSettings.DividendShareTickers;

            _candleData = await dataService.GetCandleDataAsync(_tickers);
            _candleData.TryAdd(KnownTickers.MON, await dataService.GetMoneyEquivalentCandlesAsync(_from, _to));
            _instrumentData = await dataService.GetInstrumentDataAsync(_tickers);

            await strategyExecuteResultRepository.DeleteAsync();

            await BacktestByStrategyName(nameof(FollowTrendUltimateSmootherStrategy));

            return new ();
        }

        private async Task BacktestByStrategyName(string strategyName)
        {
            var followTrendSettings = options.Value;

            var strategy = serviceProvider.GetRequiredKeyedService<FollowTrendStrategy>(strategyName);

            strategy.Name = strategyName;

            List<int> ParameterListPeriod = PARAMETER_LIST_PERIOD;           

            strategy.From = _from;
            strategy.To = _to;

            strategy.CandleData = _candleData;            
            strategy.PositionData = _tickers.ToDictionary(k => k, v => new PositionData { Ticker = v, Lot = _instrumentData[v].Lot ?? 1 });
            strategy.PositionData.TryAdd(KnownTickers.MON, new PositionData { Ticker = KnownTickers.MON, Lot = 1 });

            strategy.TradingProcessor = new TradingProcessor
            {
                CandleData = _candleData,
                InstrumentData = _instrumentData,
                Tickers = _tickers,
                StartMoneySum = followTrendSettings.StartMoneySum
            };

            foreach (var period in ParameterListPeriod)
            {
                var strategyParams =
                        $"Period = {JsonSerializer.Serialize(period)};"
                        ;                        

                try
                {
                    strategy.Period = period;

                    strategy.StartMoneySum = followTrendSettings.StartMoneySum;

                    strategy.EquitySeries.Data.Clear();
                    strategy.DrawdownSeries.Data.Clear();
                    strategy.MoneySeries.Data.Clear();

                    strategy.TradingProcessor.Reset();

                    strategy.Execute();

                    var strategyExecuteResult = ToStrategyExecuteResult(strategy);

                    strategyExecuteResult.StrategyParams = strategyParams;
                    strategyExecuteResult.ResultMessage = "OK";

                    await strategyExecuteResultRepository.AddAsync([strategyExecuteResult]);
                }

                catch (Exception ex)
                {
                    var strategyExecuteResult = new StrategyExecuteResult
                    {
                        StrategyParams = strategyParams,
                        ResultMessage = $"Error. {ex.Message}"
                    };

                    await strategyExecuteResultRepository.AddAsync([strategyExecuteResult]);
                }
            }
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
