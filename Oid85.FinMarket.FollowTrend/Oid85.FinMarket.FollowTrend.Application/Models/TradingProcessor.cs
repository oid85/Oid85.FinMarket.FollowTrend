using System.Drawing;
using System.Timers;
using Oid85.FinMarket.FollowTrend.Core.Models;
using static Oid85.FinMarket.FollowTrend.Common.KnownConstants.KnownTickers;

namespace Oid85.FinMarket.FollowTrend.Application.Models
{
    public class TradingProcessor
    {
        public double Leverage { get; set; } = 1.0;

        public DateOnly CurrentDate { get; set; } = DateOnly.MinValue;
        
        public double StartMoneySum { get; set; } = 0.0;
        
        public Dictionary<string, List<Candle>> CandleData { get; set; } = [];
        
        public Dictionary<string, Instrument> InstrumentData { get; set; } = [];

        public Dictionary<string, Balance> BalanceData { get; set; } = [];

        public List<string> Tickers { get; set; } = [];
                       
        public double TotalSum => BalanceData.Sum(x => x.Value.Cost);

        /// <summary>
        /// Сбросить настройки процессора
        /// </summary>
        public void Reset()
        {
            BalanceData = Tickers.ToDictionary(k => k, v => new Balance());
            BalanceData.TryAdd(MON, new Balance());
            BalanceData.TryAdd(RUB, new Balance { Price = 1.0, Size = StartMoneySum });
        }

        /// <summary>
        /// Синхронизировать позиции в портфеле по заданным весам активов
        /// </summary>        
        public void SyncPositions(Dictionary<string, double> weightData, bool openPositionMon = true)
        {
            // Закрываем все позиции
            ClosePositions();

            // Открываем позиции по заданным весам активов
            OpenPositionsByWeight(weightData);

            // Закупаем на остатки фонд ликвидности
            if (openPositionMon)
                OpenPositionMon();
        }

        /// <summary>
        /// Открыть позиции по весам
        /// </summary>
        public void OpenPositionsByWeight(Dictionary<string, double> weightData)
        {
            double weightSum = weightData.Sum(x => x.Value);

            foreach (var (ticker, _) in weightData.Where(x => x.Key != MON).Where(x => x.Value > 0.0).ToDictionary())
            {
                double share = weightData[ticker] / weightSum;
                OpenPositionByShare(ticker, share);
            }            
        }

        /// <summary>
        /// Открыть позицию по тикеру на долю портфеля
        /// </summary>
        public void OpenPositionByShare(string ticker, double share)
        {
            // Расчет позиции
            double cost = TotalSum * share;
            double price = BalanceData[ticker].Price;
            int lot = InstrumentData[ticker].Lot ?? 1;
            double size = Math.Truncate(cost / price / lot) * lot;
            double realCost = size * price;

            // Запоминаем цену входа в позицию
            BalanceData[ticker].EntryPrice = price;

            // Увеличиваем позицию актива
            BalanceData[ticker].Size += size;

            // Списываем деньги
            BalanceData[RUB].Size -= realCost;
        }

        /// <summary>
        /// Закрыть позиции по активам
        /// </summary>
        public void ClosePositions()
        {
            foreach (var (ticker, _) in BalanceData.Where(x => x.Key != RUB).ToDictionary())
            {
                // Расчет профита позиции
                BalanceData[ticker].Profit = BalanceData[ticker].EntryPrice.HasValue 
                    ? (BalanceData[ticker].Price - BalanceData[ticker].EntryPrice!.Value) * BalanceData[ticker].Size
                    : 0.0;

                if (ticker != MON)
                    BalanceData[ticker].Profit *= Leverage;

                // Расчет стоимости входа в позицию
                double entryCost = BalanceData[ticker].EntryPrice.HasValue
                    ? BalanceData[ticker].EntryPrice!.Value * BalanceData[ticker].Size 
                    : 0.0;

                // Расчет результирующей стоимости
                double cost = entryCost + BalanceData[ticker].Profit;

                // Обнуляем цену входа в позицию
                BalanceData[ticker].EntryPrice = null;

                // Переводим деньги от продажи актива в рубли
                BalanceData[RUB].Size += cost;

                // Обнуляем позицию по активу
                BalanceData[ticker].Size = 0.0;
            }
        }

        /// <summary>
        /// Закупить на остаток фонд ликвидности
        /// </summary>       
        public void OpenPositionMon()
        {
            // Закупаем на остатки фонд ликвидности
            double cost = BalanceData[RUB].Cost;
            double price = BalanceData[MON].Price;
            double size = Math.Truncate(cost / price);
            double realCost = size * price;

            // Запоминаем цену входа в позицию
            BalanceData[MON].EntryPrice = BalanceData[MON].Price;

            // Увеличиваем позицию актива
            BalanceData[MON].Size += size;

            // Списываем деньги
            BalanceData[RUB].Size -= realCost;
        }

        /// <summary>
        /// Обновить цены активов
        /// </summary>
        public void UpdatePrices()
        {
            foreach (var (ticker, _) in BalanceData.Where(x => x.Key != RUB).ToDictionary())
                BalanceData[ticker].Price = CandleData[ticker].FindLast(x => x.Date <= CurrentDate.AddDays(-1))?.Close ?? 0.0;
        }

        /// <summary>
        /// Обновить цены активов (недельный период)
        /// </summary>
        public void UpdateWeekPrices()
        {
            foreach (var (ticker, _) in BalanceData.Where(x => x.Key != RUB).ToDictionary())
                BalanceData[ticker].Price = CandleData[ticker].FindLast(x => x.Date <= CurrentDate.AddDays(-7))?.Close ?? 0.0;
        }
    }
}
