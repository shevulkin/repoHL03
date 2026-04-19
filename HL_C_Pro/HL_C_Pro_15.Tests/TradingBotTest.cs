using Moq;


namespace HL_C_Pro_15.Tests
{
    [TestClass]
    public sealed class TradingBotTest
    {
        [TestMethod]
        public void Test_Should_Buy()
        {
            // Arrange
            var mock = new Mock<IExchangeService>();
            mock.Setup(s => s.GetCurrentPrice("BTC")).Returns(85m);
            var bot = new TradingBot(mock.Object);

            // Act
            var result = bot.ExecuteStrategy("BTC", 100m);

            // Assert
            Assert.AreEqual("Buy", result);
        }

        [TestMethod]
        public void Test_Should_Sell()
        {
            // Arrange
            var mock = new Mock<IExchangeService>();
            mock.Setup(s => s.GetCurrentPrice("BTC")).Returns(115m);
            var bot = new TradingBot(mock.Object);

            // Act
            var result = bot.ExecuteStrategy("BTC", 100m);

            // Assert
            Assert.AreEqual("Sell", result);
        }

        [TestMethod]
        public void Test_Should_Hold()
        {
            // Arrange
            var mock = new Mock<IExchangeService>();
            mock.Setup(s => s.GetCurrentPrice("BTC")).Returns(95m);
            var bot = new TradingBot(mock.Object);

            // Act
            var result = bot.ExecuteStrategy("BTC", 100m);

            // Assert
            Assert.AreEqual("Hold", result);
        }
    }
}
