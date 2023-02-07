using RebalanceStockPortfolio.Interfaces;
using System.Net;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RebalanceStockPortfolio.Services
{
    public class StockPriceService : IStockPrice
    {
        public StockPriceService() { }

        public decimal getClosedStockPrice(string symbol)
        {

            string QUERY_URL = $"https://www.alphavantage.co/query?apikey=BPYXEC00NPEI4YSY&function=TIME_SERIES_DAILY_ADJUSTED&symbol={symbol}";
            Uri queryUri = new Uri(QUERY_URL);

            using (HttpClient client = new HttpClient())
            {
                Task<string> result = client.GetStringAsync(queryUri);

                dynamic json_data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result.Result)!;

                var test = new System.Collections.Generic.Dictionary<string, object>(json_data).ToArray();

                var test1 = ((Newtonsoft.Json.Linq.JContainer)test[1].Value).FirstOrDefault();

                Newtonsoft.Json.Linq.JToken? jproperty = test1?.FirstOrDefault();  

                foreach (Newtonsoft.Json.Linq.JProperty t in jproperty)
                {
                    if (t.Name.Contains("4. close"))
                    {
                        return decimal.Parse(t.Value.ToString());
                    }
                }
              
            }
            return decimal.Parse("0");
        }

        public Stock[] CalculateRebalancing(Stock[] stocks)
        {           

            return stocks;
        }
    }
}
