using Oid85.FinMarket.FollowTrend.Application.Interfaces.ApiClients;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Services;
using Oid85.FinMarket.FollowTrend.Common.Utils;
using Oid85.FinMarket.FollowTrend.Core.Models;

namespace Oid85.FinMarket.FollowTrend.Application.Services
{
    public class DataService(
        IStorageApiClient storageApiClient) 
        : IDataService
    {
        private Dictionary<string, List<Candle>>? _candleData = null;
        private Dictionary<string, List<DateValue<double>>>? _ultimateSmootherData = null;
        private Dictionary<string, Instrument>? _instrumentData = null;

        public async Task<Dictionary<string, List<Candle>>> GetCandleDataAsync(List<string> tickers)
        {
            if (_candleData is not null)
            {
                foreach (var ticker in tickers)                
                    if (!_candleData.ContainsKey(ticker))
                        _candleData.Add(ticker, await GetCandlesByTickerAsync(ticker));
                
                return _candleData;
            }

            _candleData = [];

            foreach (var ticker in tickers.Distinct()) 
                _candleData.Add(ticker, await GetCandlesByTickerAsync(ticker));

            return _candleData;
        }

        public async Task<Dictionary<string, List<DateValue<double>>>> GetUltimateSmootherDataAsync(List<string> tickers, int period)
        {
            if (_ultimateSmootherData is not null)
            {
                foreach (var ticker in tickers)
                    if (!_ultimateSmootherData.ContainsKey(ticker))
                    {
                        var candles = await GetCandlesByTickerAsync(ticker);
                        _ultimateSmootherData.Add(ticker, GetUltimateSmootherValues(candles, period));
                    }
                
                return _ultimateSmootherData;
            }    

            _ultimateSmootherData = [];

            var candleData = await GetCandleDataAsync(tickers);

            foreach (var ticker in tickers.Distinct())
                _ultimateSmootherData.Add(ticker, GetUltimateSmootherValues(candleData[ticker], period));

            return _ultimateSmootherData;
        }

        public async Task<Dictionary<string, Instrument>> GetInstrumentDataAsync(List<string> tickers)
        {
            if (_instrumentData is not null)
                if (tickers.All(x => _instrumentData.ContainsKey(x)))
                    return _instrumentData;

            _instrumentData = (await storageApiClient.GetInstrumentListAsync(new())).Result.Instruments
                .Where(x => tickers.Distinct().Contains(x.Ticker))
                .ToDictionary(
                k => k.Ticker,
                v => new Instrument
                {
                    Ticker = v.Ticker,
                    Name = v.Name,
                    Type = v.Type,
                    Lot = v.Lot
                });

            return _instrumentData;
        }

        public async Task<List<Candle>> GetMoneyEquivalentCandlesAsync(DateOnly from, DateOnly to)
        {
            var dates = DateUtils.GetDates(from.AddDays(-100), to);

            double price = 100.0;

            var candles = new List<Candle>
            {
                new() {
                    Date = dates[0],
                    Open = price,
                    Close = price,
                    High = price,
                    Low = price,
                    Volume = 0
                }
            };

            double keyRate = 12.0;
            double dayPercentRate = keyRate / 365.0;
            double multipleCoefficient = 1.0 + dayPercentRate / 100.0;

            for (int i = 1; i < dates.Count; i++)
            {
                candles.Add(
                    new()
                    {
                        Date = dates[i],
                        Open = candles[i - 1].Open * multipleCoefficient,
                        Close = candles[i - 1].Close * multipleCoefficient,
                        High = candles[i - 1].High * multipleCoefficient,
                        Low = candles[i - 1].Low * multipleCoefficient,
                        Volume = 0
                    });
            }

            return candles;
        }

        private async Task<List<Candle>> GetCandlesByTickerAsync(string ticker)
        {
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var response = await storageApiClient.GetCandleListAsync(
                new()
                {
                    From = from,
                    To = to,
                    Ticker = ticker
                });

            var candles =  response.Result.Candles
                .Select(x =>
                new Candle
                {
                    Open = x.Open,
                    Close = x.Close,
                    Low = x.Low,
                    High = x.High,
                    Volume = x.Volume,
                    Date = x.Date
                })
                .OrderBy(x => x.Date)
                .ToList();

            for (int i = 0; i < candles.Count; i++) candles[i].Index = i;

            return candles;
        }

        private static List<DateValue<double>> GetUltimateSmootherValues(List<Candle> candles, int period)
        {
            // Ultimate Smoother function based on John Ehlers' formula
            double coeff = Math.Sqrt(2.0);
            double step = 2.0 * Math.PI / period;
            double a1 = Math.Exp(-1.0 * coeff * Math.PI / period);
            double b1 = 2.0 * a1 * Math.Cos(coeff * step / period);
            double c2 = b1;
            double c3 = -1.0 * a1 * a1;
            double c1 = (1 + c2 - c3) / 4.0;

            var result = new List<DateValue<double>>();

            for (int i = 0; i < candles.Count; i++)
                if (i < 3)
                    result.Add(
                        new DateValue<double>
                        {
                            Date = candles[i].Date,
                            Value = candles[i].Close
                        });

                else
                    result.Add(
                        new DateValue<double>
                        {
                            Date = candles[i].Date,
                            Value = (1 - c1) * candles[i].Close +
                                    (2 * c1 - c2) * candles[i - 1].Close -
                                    (c1 + c3) * candles[i - 2].Close +
                                    c2 * result[i - 1].Value +
                                    c3 * result[i - 2].Value
                        });

            return result;
        }
    }
}
