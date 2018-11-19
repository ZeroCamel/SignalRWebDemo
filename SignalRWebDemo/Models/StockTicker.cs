using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace SignalRWebDemo.Models
{
    public class StockTicker
    {
        private static readonly Lazy<StockTicker> _instance = new Lazy<StockTicker>(() => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));

        /// <summary>
        /// 1、ConcurrentDictionary 可以由多个线程访问的安全集合
        /// 1、存放股票代码和股票价格
        /// </summary>
        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();
        private IHubConnectionContext<dynamic> Clients { get; set; }

        //锁
        private readonly object _updateaStockPriceLock = new object();

        private readonly double _rangePercent = 1;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private readonly Timer _timer;
        private volatile bool _UpdateStockPrices = false;
        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            _stocks.Clear();

            var stocks = new List<Stock>()
            {
                new Stock { Symbol="MSFT",Price=30.31m},
                new Stock {Symbol="APPL",Price= 578.18m},
                new Stock { Symbol="AMPS",Price=570.30m}
            };

            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));

            _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);
        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        /// <summary>
        /// 1、多个线程访问当前类 保证线程安全
        /// </summary>
        /// <param name="state"></param>
        private void UpdateStockPrices(object state)
        {
            lock (_updateaStockPriceLock)
            {
                if (!_UpdateStockPrices)
                {
                    _UpdateStockPrices = true;
                    foreach (var stock in _stocks.Values)
                    {
                        if (TryUpdateStockPrice(stock))
                        {
                            BroadCastStockPrice(stock);
                        }
                    }
                    _UpdateStockPrices = false;
                }
            }
        }

        private bool TryUpdateStockPrice(Stock stock)
        {
            double a = _updateOrNotRandom.NextDouble();
            if (a > .1)
            {
                return false;
            }
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > .51;
            var change = Math.Round(stock.Price*(decimal)percentChange,2);
            change = pos ? change : -change;
            stock.Price += change;
            return true;
        }

        private void BroadCastStockPrice(Stock stock)
        {
            Clients.All.updateStockPrice(stock);
        }
    }
}