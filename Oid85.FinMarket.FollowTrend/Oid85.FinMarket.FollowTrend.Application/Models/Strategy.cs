using Oid85.FinMarket.FollowTrend.Application.Helpers;
using Oid85.FinMarket.FollowTrend.Common.Extensions;
using Oid85.FinMarket.FollowTrend.Common.KnownConstants;
using Oid85.FinMarket.FollowTrend.Common.Utils;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Models
{
    public class Strategy
    {
        #region Блок параметров торгового алгоритма

        public string Timeframe { get; set; } = KnownTimeframes.Day;

        public int Period { get; set; } = 10;

        public double Multiplier { get; set; } = 3;

        public int CountTickers { get; set; } = 10;

        public List<int> RebalanceDays { get; set; } = [1, 11, 21];

        #endregion

        public virtual string Name { get; set; } = string.Empty;

        public virtual List<string> GetDescription() => [];

        public DateOnly From { get; set; } = new DateOnly(2021, 1, 1);

        public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public List<DateOnly> Dates => DateUtils.GetDates(From, To);

        public List<(DateOnly StartDate, DateOnly EndDate)> Weeks => DateUtils.GetWeeks(From, To);

        /// <summary>
        /// Актуальная сумма. Используется для расчета текущих позиций
        /// </summary>
        public double TotalSumLife { get; set; } = 0.0;

        /// <summary>
        /// Начальная сумма капитала. Используется в бэктесте
        /// </summary>
        public double StartMoneySum { get; set; } = 0.0;

        /// <summary>
        /// Конечная сумма капитала. Используется в бэктесте
        /// </summary>
        public double EndMoneySum => EquitySeries.Data is [] ? StartMoneySum : EquitySeries.Data.Last().Value ?? StartMoneySum;

        /// <summary>
        /// Процессор обработки торговых операций
        /// </summary>
        public TradingProcessor TradingProcessor { get; set; } = new();

        /// <summary>
        /// Текущий срез позиций
        /// </summary>
        public Dictionary<string, PositionData> PositionData { get; set; } = [];

        /// <summary>
        /// История позиций
        /// </summary>
        public Dictionary<string, List<PositionData>> PositionDataList { get; set; } = [];

        public Dictionary<string, List<Candle>> CandleData { get; set; } = [];

        public Dictionary<string, List<DateValue<double>>> UltimateSmootherData { get; set; } = [];

        public Dictionary<string, List<DateValue<double?>>> SupertrendLowerBandData { get; set; } = [];

        public Dictionary<string, List<Candle>> WeekCandleData { get; set; } = [];

        public Dictionary<string, List<DateValue<double>>> WeekUltimateSmootherData { get; set; } = [];

        public Dictionary<string, List<DateValue<double?>>> WeekSupertrendLowerBandData { get; set; } = [];

        public List<FollowTrendMessage> Messages { get; set; } = [];

        public DiagramSeries EquitySeries { get; set; } = new() { Name = "Капитал", Color = KnownColors.Green, ColorFill = KnownColors.Green };

        public DiagramSeries MoneySeries { get; set; } = new() { Name = "Фонд ликвидности", Color = KnownColors.LightBlue, ColorFill = KnownColors.LightBlue };

        public DiagramSeries ShortEquitySeries => DiagramSeriesHelper.GetShortEquitySeries(EquitySeries);

        public DiagramSeries DrawdownSeries => DiagramSeriesHelper.GetDrawdownSeries(EquitySeries, false);

        public DiagramSeries DrawdownSeriesPercent => DiagramSeriesHelper.GetDrawdownSeries(EquitySeries, true);

        public double MaxDrawdownPercent => DrawdownSeriesPercent.Data.Where(x => x.Value.HasValue).Min(x => x.Value!.Value).RoundTo(2);

        public double MaxDrawdown => DrawdownSeries.Data.Where(x => x.Value.HasValue).Min(x => x.Value!.Value).RoundTo(2);

        public double RecoveryFactor => MaxDrawdown == 0.0 ? double.PositiveInfinity : Math.Abs(NetProfit / MaxDrawdown).RoundTo(2);

        public double NetProfit => EndMoneySum > StartMoneySum ? (EndMoneySum - StartMoneySum).RoundTo(2) : 0.0;

        public double TotalReturn => EndMoneySum > StartMoneySum ? ((EndMoneySum - StartMoneySum) / StartMoneySum * 100.0).RoundTo(2) : 0.0;

        public double AnnualYieldReturn => EndMoneySum > StartMoneySum ? (TotalReturn / ((To.DayNumber - From.DayNumber) / 365.0)).RoundTo(2) : 0.0;

        /// <summary>
        /// Все тикеры
        /// </summary>
        public List<string> Tickers => [.. PositionData.Keys];

        /// <summary>
        /// Тикеры, у которых есть сигнал на покупку
        /// </summary>
        public List<string> SignalTickers { get; set; } = [];

        /// <summary>
        /// ТИкеры портфеля
        /// </summary>
        public List<string> PortfolioTickers => [.. PositionData.Values.Where(x => x.Weight > 0.0).Select(x => x.Ticker)];

        /// <summary>
        /// ТИкеры портфеля (без фонда ликвидности)
        /// </summary>
        public List<string> PortfolioWithoutMonTickers => [.. PositionData.Values.Where(x => x.Ticker != MON).Where(x => x.Weight > 0.0).Select(x => x.Ticker)];

        public double WeightSum => PositionData.Values.Sum(x => x.Weight);

        public DateOnly CurrentDate { get; set; } = DateOnly.MinValue;        

        public virtual void Execute()
        {

        }

        public virtual void InitParameters()
        {

        }

        public void AddMessage(string ticker, string message, string colorFill) => Messages.Add(new() { Date = CurrentDate, Ticker = ticker, Message = message, ColorFill = colorFill });

        public void ClearMessages() => Messages.Clear();

        public Candle? GetCandle(string ticker) => CandleData[ticker].FindLast(x => x.Date <= CurrentDate.AddDays(-1));

        public Candle? GetWeekCandle(string ticker) => WeekCandleData[ticker].FindLast(x => x.Date <= CurrentDate.AddDays(-7));

        public virtual void SetSignalTickers()
        {

        }

        public void SetWeights()
        {
            Tickers.ForEach(ticker => PositionData[ticker].Weight = 0.0);
            SignalTickers.ForEach(ticker => PositionData[ticker].Weight = 1.0);
            PositionData[MON].Weight = Tickers.Count - SignalTickers.Count(x => x != MON);
        }

        public void UpdatePrices() => TradingProcessor.UpdatePrices();

        public void UpdateWeekPrices() => TradingProcessor.UpdateWeekPrices();

        public void UpdateCandles()
        {
            foreach (var ticker in Tickers)
            {
                var candle = GetCandle(ticker) ?? new Candle();

                PositionData[ticker].Candle.Date = candle.Date;
                PositionData[ticker].Candle.Open = candle.Open;
                PositionData[ticker].Candle.Close = candle.Close;
                PositionData[ticker].Candle.High = candle.High;
                PositionData[ticker].Candle.Low = candle.Low;
                PositionData[ticker].Candle.Volume = candle.Volume;
            }
        }

        public void UpdateWeekCandles()
        {
            foreach (var ticker in Tickers)
            {
                var candle = GetWeekCandle(ticker) ?? new Candle();

                PositionData[ticker].Candle.Date = candle.Date;
                PositionData[ticker].Candle.Open = candle.Open;
                PositionData[ticker].Candle.Close = candle.Close;
                PositionData[ticker].Candle.High = candle.High;
                PositionData[ticker].Candle.Low = candle.Low;
                PositionData[ticker].Candle.Volume = candle.Volume;
            }
        }

        /// <summary>
        /// Синхронизация позиций
        /// </summary>
        public void SyncPositions() => TradingProcessor.SyncPositions(PositionData.ToDictionary(k => k.Key, v => v.Value.Weight), true);

        /// <summary>
        /// Обновление кривой капитала
        /// </summary>
        public void UpdateEquitySeries() =>
            EquitySeries.Data.Add(
                new()
                {
                    Date = CurrentDate,
                    Value = TradingProcessor.TotalSum.RoundTo(2)
                });

        /// <summary>
        /// Обновление кривой фонда ликвидности
        /// </summary>
        public void UpdateMoneySeries() =>
            MoneySeries.Data.Add(
                new()
                {
                    Date = CurrentDate,
                    Value = (TradingProcessor.BalanceData[RUB].Cost + TradingProcessor.BalanceData[MON].Cost).RoundTo(2)
                });

        public double GetCurrentDrawdown()
        {
            var equityValues = EquitySeries.Data.Select(x => x.Value ?? 0.0).ToList();

            if (equityValues.Count == 0) return 0.0;

            double lastEquity = equityValues[^1];
            double maxEquity = equityValues.Max();

            if (maxEquity == 0.0) return 0.0;

            return -1 * Math.Abs((maxEquity - lastEquity) / maxEquity * 100.0).RoundTo(2);
        }

        public List<CurrentPosition> GetCurrentPositions()
        {
            var currentPositions = new List<CurrentPosition>();

            foreach (var ticker in PortfolioTickers)
            {
                var candle = Timeframe switch
                {
                    KnownTimeframes.Day => CandleData[ticker].FindLast(x => x.Date < Dates[^1]),
                    KnownTimeframes.Week => WeekCandleData[ticker].FindLast(x => x.Date < Weeks[^1].StartDate),
                    _ => null
                };

                double tickerCost = TotalSumLife / WeightSum * PositionData[ticker].Weight.RoundTo(2);
                int tickerSize = Convert.ToInt32(Math.Truncate(tickerCost / candle!.Close / PositionData[ticker].Lot) * PositionData[ticker].Lot);
                double entryPrice = ticker == MON ? 0.0 : PositionData[ticker].EntryPrice;
                double profitPercent = ticker == MON ? 0.0 : PositionData[ticker].ProfitPercent;

                currentPositions.Add(
                    new CurrentPosition
                    {
                        Ticker = ticker,
                        Weight = PositionData[ticker].Weight,
                        Size = tickerSize,
                        Cost = tickerCost,
                        EntryPrice = entryPrice,
                        ProfitPercent = profitPercent
                    });
            }

            List<CurrentPosition> orderedCurrentPositions = [
                    .. currentPositions.Where(x => x.Ticker != MON).OrderBy(x => x.Ticker),
                    .. currentPositions.Where(x => x.Ticker == MON)
                    ];
            
            for (int i = 0; i < orderedCurrentPositions.Count; i++)
                orderedCurrentPositions[i].Number = i + 1;

            return orderedCurrentPositions;
        }

        public List<DiagramSeries> GetPriceDynamicSeries() =>
            Timeframe switch
            {
                KnownTimeframes.Day => DiagramSeriesHelper.GetPriceDynamicSeries(PortfolioTickers, CandleData, Period),
                KnownTimeframes.Week => DiagramSeriesHelper.GetPriceDynamicSeries(PortfolioTickers, WeekCandleData, Period),
                _ => []
            };

        public List<DiagramSeries> GetPriceSeries() =>
            Timeframe switch
            {
                KnownTimeframes.Day => DiagramSeriesHelper.GetPriceSeries(PortfolioTickers, CandleData),
                KnownTimeframes.Week => DiagramSeriesHelper.GetPriceSeries(PortfolioTickers, WeekCandleData),
                _ => []
            };

        public List<List<DiagramSeries>> GetPriceWithEntryPriceSeries() =>
            Timeframe switch
            {
                KnownTimeframes.Day => DiagramSeriesHelper.GetPriceWithEntryPriceSeries(PortfolioTickers, CandleData, PositionData),
                KnownTimeframes.Week => DiagramSeriesHelper.GetPriceWithEntryPriceSeries(PortfolioTickers, WeekCandleData, PositionData),
                _ => []
            };
    }
}
