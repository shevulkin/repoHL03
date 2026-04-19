var exchange = new FakeExchangeService();
var res = new TradingBot(exchange).ExecuteStrategy("BTCUSD", 1000);
Console.WriteLine(res);

public class FakeExchangeService : IExchangeService
{
    private static readonly Random _rng = new();
    public decimal GetCurrentPrice(string symbol) =>
        _rng.Next(800, 1201);
}

public interface IExchangeService
{
    decimal GetCurrentPrice(string symbol);
}

public class TradingBot
{
    private readonly IExchangeService _exchange;

    public TradingBot(IExchangeService exchange)
    {
        _exchange = exchange;
    }

    public string ExecuteStrategy(string symbol, decimal averagePrice)
    {
        var currentPrice = _exchange.GetCurrentPrice(symbol);

        if (currentPrice <= averagePrice * 0.9m) return "Buy";
        if (currentPrice >= averagePrice * 1.1m) return "Sell";

        return "Hold";
    }
}
