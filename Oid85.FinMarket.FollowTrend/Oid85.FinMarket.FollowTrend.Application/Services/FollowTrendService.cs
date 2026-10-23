using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.ApiClients;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Repositories;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Services;
using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Core.Configuration;
using Oid85.FinMarket.FollowTrend.Core.Models;
using Oid85.FinMarket.FollowTrend.Core.Requests;
using Oid85.FinMarket.FollowTrend.Core.Responses;
using static Oid85.FinMarket.FollowTrend.Application.Mapping.ApplicationMapper;
using static Oid85.FinMarket.FollowTrend.Application.Mapping.TerminalMapper;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Services
{
    public class FollowTrendService(
        IOptions<FollowTrendSettings> options,
        IDataService dataService,
        IParameterRepository parameterRepository,
        ITraderFinamApiClient traderFinamApiClient,
        IServiceProvider serviceProvider)
        : IFollowTrendService
    {
        public async Task<MonitorResponse> MonitorVersionAsync(MonitorRequest request)
        {
            request.FollowTrendVersion = "UltimateSmoother";

            var followTrendSettings = options.Value;

            var tickers = followTrendSettings.DividendShareTickers;

            var strategy = serviceProvider.GetRequiredKeyedService<FollowTrendStrategy>($"FollowTrend{request.FollowTrendVersion}Strategy");

            strategy.InitMonitorParameters();

            strategy.StartMoneySum = followTrendSettings.StartMoneySum;         

            var from = new DateOnly(2021, 2, 1);
            var to = DateOnly.FromDateTime(DateTime.Today);

            strategy.From = from;
            strategy.To = to;

            var candleData = await dataService.GetCandleDataAsync(tickers);
            candleData.TryAdd(MON, await dataService.GetMoneyEquivalentCandlesAsync(from, to));
            strategy.CandleData = candleData;

            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers, strategy.Period);
            strategy.UltimateSmootherData = ultimateSmootherData;

            var instrumentData = await dataService.GetInstrumentDataAsync(tickers);
            strategy.PositionData = tickers.ToDictionary(k => k, v => new PositionData { Ticker = v, Weight = 0, Lot = instrumentData[v].Lot ?? 1 });
            strategy.PositionData.TryAdd(MON, new PositionData { Ticker = MON, Weight = 0, Lot = 1 });

            strategy.TotalSumLife = Convert.ToDouble(((await parameterRepository.GetParameterValueAsync("TotalSum:FollowTrend")) ?? "0").Replace(" ", "").Trim());
            
            strategy.TradingProcessor = new TradingProcessor
            {
                CandleData = candleData,
                InstrumentData = instrumentData,
                Tickers = tickers,
                StartMoneySum = followTrendSettings.StartMoneySum
            };

            strategy.TradingProcessor.Reset();

            strategy.Execute();

            return ToMonitorResponse(strategy);
        }

        public async Task<EditPortfolioTotalSumResponse> EditPortfolioTotalSumAsync(EditPortfolioTotalSumRequest request)
        {
            await parameterRepository.SetParameterValueAsync($"TotalSum:FollowTrend", request.TotalSum.ToString("N0"));
            return new();
        }

        public async Task<TerminalResponse> TerminalAsync(TerminalRequest request)
        {
            var portfolioInfoResult = (await traderFinamApiClient.GetPortfolioInfoAsync(new())).Result;
            var outboxTaskListResult = (await traderFinamApiClient.GetOutboxTaskListAsync(new())).Result;

            double totalSumLife = Convert.ToDouble(portfolioInfoResult.TotalSum);

            await EditPortfolioTotalSumAsync(new EditPortfolioTotalSumRequest { TotalSum = totalSumLife });

            var monitorResponse = await MonitorVersionAsync(new MonitorRequest { FollowTrendVersion = request.FollowTrendVersion });

            if (monitorResponse.CurrentPositions.Select(x => x.Ticker).Contains(MON))
                monitorResponse.CurrentPositions.Find(x => x.Ticker == MON)!.Ticker = FMMM;

            return ToTerminalResponse(monitorResponse, portfolioInfoResult, outboxTaskListResult);
        }
    }
}
