namespace RebalanceStockPortfolio.Interfaces
{
    public interface IStockPrice
    {
        decimal getClosedStockPrice(string symbol);

        Stock[] CalculateRebalancing(Stock[] stocks);
    }
}
