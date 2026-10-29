using Oid85.FinMarket.FollowTrend.Application.Helpers;
using Oid85.FinMarket.FollowTrend.Application.Models;
using Oid85.FinMarket.FollowTrend.Core.Models;
using Oid85.FinMarket.FollowTrend.Core.Responses;
using Skender.Stock.Indicators;

namespace Oid85.FinMarket.FollowTrend.Application.Mapping;

public static class ApplicationMapper
{
    public static MonitorResponse ToMonitorResponse(Strategy strategy) => 
        new ()
        {
            Description = strategy.GetDescription(),
            Messages = [.. strategy.Messages.OrderByDescending(x => x.Date)],
            TotalSumLife = strategy.TotalSumLife,
            BacktestSeries = [strategy.EquitySeries, strategy.MoneySeries, strategy.DrawdownSeries],
            ShortBacktestSeries = [strategy.ShortEquitySeries],
            PriceDynamicSeries = strategy.GetPriceDynamicSeries(),
            PriceSeries = strategy.GetPriceSeries(),
            PriceWithEntryPriceSeries = strategy.GetPriceWithEntryPriceSeries(),
            CurrentPositions = strategy.GetCurrentPositions(),
            Yield = strategy.AnnualYieldReturn,
            Yield2021 = DiagramSeriesHelper.GetYearPercentageYield(strategy.EquitySeries, 2021),
            Yield2022 = DiagramSeriesHelper.GetYearPercentageYield(strategy.EquitySeries, 2022),
            Yield2023 = DiagramSeriesHelper.GetYearPercentageYield(strategy.EquitySeries, 2023),
            Yield2024 = DiagramSeriesHelper.GetYearPercentageYield(strategy.EquitySeries, 2024),
            Yield2025 = DiagramSeriesHelper.GetYearPercentageYield(strategy.EquitySeries, 2025),
            Yield2026 = DiagramSeriesHelper.GetYearPercentageYield(strategy.EquitySeries, 2026),
            YieldYear = DiagramSeriesHelper.GetPercentageYield(strategy.EquitySeries, 365),
            YieldQuarter = DiagramSeriesHelper.GetPercentageYield(strategy.EquitySeries, 90),
            YieldMonth = DiagramSeriesHelper.GetPercentageYield(strategy.EquitySeries, 30),
            YieldWeek = DiagramSeriesHelper.GetPercentageYield(strategy.EquitySeries, 7),
            MaxDrawdownPercent = strategy.MaxDrawdownPercent,
            CurrentDrawdownPercent = strategy.GetCurrentDrawdown()
        };

    public static StrategyExecuteResult ToStrategyExecuteResult(Strategy strategy) =>
        new()
        {
            StrategyName = strategy.Name,
            StartDate = strategy.From,
            EndDate = strategy.To,
            Tickers = strategy.Tickers,
            RecoveryFactor = strategy.RecoveryFactor,
            NetProfit = strategy.NetProfit,
            MaxDrawdownPercent = strategy.MaxDrawdownPercent,
            StartMoney = strategy.StartMoneySum,
            EndMoney = strategy.EndMoneySum,
            TotalReturn = strategy.TotalReturn,
            AnnualYieldReturn = strategy.AnnualYieldReturn,
            EquityCurve = strategy.EquitySeries.Data
        };

    public static Quote Map(Candle model) =>
        new()
        {
            Open = Convert.ToDecimal(model.Open),
            Close = Convert.ToDecimal(model.Close),
            High = Convert.ToDecimal(model.High),
            Low = Convert.ToDecimal(model.Low),
            Date = model.Date.ToDateTime(TimeOnly.MinValue)
        };
}