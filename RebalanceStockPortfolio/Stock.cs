namespace RebalanceStockPortfolio
{
    public class Stock
    {
        public string Symbol { get; set; }

        public decimal DesiredPercentage { get; set; }       

        public int NumberOfShares { get; set; }

        public decimal ClosedSharePrice { get; set; }

        public decimal AssertValue { get { return NumberOfShares * ClosedSharePrice; } }

        public decimal DesiredAssertValue { get; set; }

        public decimal CurrentSharePrice { get; set; }

        public decimal CurrentAssertValue { get; set; }

        public decimal AssertDifference { get { return AssertValue - CurrentAssertValue; } }

        private decimal _currentPercentage;
        public decimal CurrentPercentage
        {
            get { return Math.Round(_currentPercentage); }
            set { _currentPercentage = value; }
        }      

        public bool Buy { get; set; }

        public bool Sell { get; set; }

        public int NoOfSharesToBuyOrSell { get; set; }
  

    }
}