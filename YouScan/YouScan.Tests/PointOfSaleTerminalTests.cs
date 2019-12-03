using System.Collections.Generic;
using Moq;
using Xunit;
using YouScan.Providers;

namespace YouScan.Tests
{
    public class PointOfSaleTerminalTests
    {
        public Mock<IPriceProvider> priceProviderMock;
        public PointOfSaleTerminal _sut;

        public PointOfSaleTerminalTests()
        {
            priceProviderMock = new Mock<IPriceProvider>();
            priceProviderMock.Setup(x => x.GetPrice("A")).Returns(new Price { Name = "A", ItemPrice = 1.25m, Volumes = new Dictionary<int, decimal> {{3,3}}});
            _sut = new PointOfSaleTerminal(priceProviderMock.Object);
        }

        [Fact]
        public void Scan_ShouldAddExistingItem()
        {
            _sut.Scan("A");

            Assert.Equal(1.25m, _sut.Balance);
        }

        [Fact]
        public void Scan_ShouldNotAddUnknownItem()
        {
            _sut.Scan("A");
            _sut.Scan("B");

            Assert.Equal(1.25m, _sut.Balance);
        }

        [Fact]
        public void Balance_ShouldCalculateVolume()
        {
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");

            Assert.Equal(3m, _sut.Balance);
        }

        [Fact]
        public void Balance_ShouldGetHighestVolume()
        {
            priceProviderMock.Setup(x => x.GetPrice("A")).Returns(new Price { Name = "A", ItemPrice = 1.25m, Volumes = new Dictionary<int, decimal> { { 3, 3 }, { 5, 1 } } });

            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");

            Assert.Equal(2.25m, _sut.Balance);
        }

        [Fact]
        public void Balance_ShouldCalculateAllItems()
        {
            priceProviderMock.Setup(x => x.GetPrice("B")).Returns(new Price { Name = "B", ItemPrice = 2m });

            _sut.Scan("A");
            _sut.Scan("B");

            Assert.Equal(3.25m, _sut.Balance);
        }

        [Fact]
        public void Cancel_ShouldReduceCount()
        {
            _sut.Scan("A");
            _sut.Cancel("A");
            _sut.Cancel("A");

            Assert.Equal(0m, _sut.Balance);
        }
    }
}
