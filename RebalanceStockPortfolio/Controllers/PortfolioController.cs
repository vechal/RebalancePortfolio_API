using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RebalanceStockPortfolio.Interfaces;
using RebalanceStockPortfolio.Services;

namespace RebalanceStockPortfolio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortfolioController : ControllerBase
    {

        private readonly ILogger<PortfolioController> _logger;
        private readonly IStockPrice _stockPriceService;

        public PortfolioController(ILogger<PortfolioController> logger, IStockPrice stockPrice)
        {
            _logger = logger;
            _stockPriceService = stockPrice;
        }

        [HttpGet(Name = "GetMyStocks")]
        public IEnumerable<Stock>? Get()
        {
            List<Stock>? stocks = null;

            using (StreamReader r = new StreamReader("Data/PortfolioData.json"))
            {
                string json = r.ReadToEnd();
                stocks = JsonConvert.DeserializeObject<List<Stock>>(json);
            }
            if (stocks != null)
            {
                foreach (var stock in stocks)
                {
                    stock.ClosedSharePrice = _stockPriceService.getClosedStockPrice(stock.Symbol);
                }
            }

            var totalAssertValue = stocks?.Sum(x => x.AssertValue);

            stocks?.ForEach(x =>
            {
                x.CurrentAssertValue = x.NumberOfShares * x.ClosedSharePrice;
                x.CurrentPercentage = x.AssertValue / totalAssertValue.Value * 100;
                x.DesiredAssertValue = totalAssertValue.Value / 100 * x.DesiredPercentage;
            });


            return stocks?.ToArray();

        }

        [HttpPost("UpdateStocks")]
        public IEnumerable<Stock> UpdateStocks([FromBody] List<Stock> stocks)
        {
            if (stocks != null)
            {

                var totalCurrentAssertValue = stocks?.Sum(x => x.NumberOfShares * x.CurrentSharePrice);

                foreach (Stock s in stocks)
                {
                    s.CurrentAssertValue = s.NumberOfShares * s.CurrentSharePrice;
                    s.DesiredAssertValue = totalCurrentAssertValue.Value / 100 * s.DesiredPercentage;

                    var difference = s.DesiredAssertValue - s.CurrentAssertValue;

                    if (difference > 0)
                    {
                        s.Buy = true;
                        s.Sell = false;

                        var numberOfStocksToBuy = Convert.ToInt16(Math.Round(difference / s.CurrentSharePrice));

                        s.NoOfSharesToBuyOrSell = numberOfStocksToBuy;

                    }
                    else if (difference < 0)
                    {
                        s.Sell = true;
                        s.Buy = false;

                        var numberOfStocksToSell = Convert.ToInt16(Math.Round(Math.Abs(difference) / s.CurrentSharePrice));

                        s.NoOfSharesToBuyOrSell = numberOfStocksToSell;
                    }
                    else
                    {
                        s.Sell = false;
                        s.Buy = false;
                    }

                }
            }

            return stocks;
        }

        [HttpPost("RebalanceStocks")]
        public IEnumerable<Stock> RebalanceStocks([FromBody] List<Stock> stocks)
        {

            if (stocks != null)
            {
                foreach (Stock s in stocks)
                {
                    if (s.Sell)
                    {
                        s.NumberOfShares = s.NumberOfShares - s.NoOfSharesToBuyOrSell;
                        s.CurrentAssertValue = s.NumberOfShares * s.CurrentSharePrice;

                    }
                    else if (s.Buy)
                    {
                        s.NumberOfShares = s.NumberOfShares + s.NoOfSharesToBuyOrSell;
                        s.CurrentAssertValue = s.NumberOfShares * s.CurrentSharePrice;
                    }
                }

                var totalCurrentAssertValue = stocks?.Sum(x => x.CurrentAssertValue);

                foreach (Stock s in stocks)
                {
                    s.CurrentPercentage = Convert.ToInt16(Math.Round(s.CurrentAssertValue / totalCurrentAssertValue.Value * 100));
                }
            }

            return stocks;

        }
    }
}