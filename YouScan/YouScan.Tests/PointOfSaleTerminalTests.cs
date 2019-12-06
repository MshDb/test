using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using YouScan.Providers;
using YouScan.Repositories;

namespace YouScan.Tests
{
    public class PointOfSaleTerminalTests
    {
        public Mock<IPriceProvider> priceProviderMock;
        public Mock<IDiscountProvider> discountProviderMock;
        public Mock<IDiscountCardRepository> discountCardRepositoryMock;

        public PointOfSaleTerminal _sut;

        public PointOfSaleTerminalTests()
        {
            priceProviderMock = new Mock<IPriceProvider>();
            priceProviderMock.Setup(x => x.GetPrice("A")).Returns(new Price { Name = "A", ItemPrice = 1.25m, Volumes = new Dictionary<int, decimal> {{3,3}}});

            discountProviderMock = new Mock<IDiscountProvider>();
            discountProviderMock.Setup(x => x.GetDiscount(It.IsAny<decimal>())).Returns(0);

            discountCardRepositoryMock = new Mock<IDiscountCardRepository>();
            discountCardRepositoryMock.Setup(x => x.Get(Guid.Empty)).Returns(1);

            _sut = new PointOfSaleTerminal(priceProviderMock.Object, discountCardRepositoryMock.Object, discountProviderMock.Object);
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
        public void Balance_ShouldApplyDiscount()
        {
            _sut.Scan("A");
            discountProviderMock.Setup(x => x.GetDiscount(It.IsAny<decimal>())).Returns(20);

            Assert.Equal(1m, _sut.Balance);
        }

        [Fact]
        public void Balance_ShouldNotApplyDiscountToProductsWithVolume()
        {
            discountProviderMock.Setup(x => x.GetDiscount(It.IsAny<decimal>())).Returns(20);
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");

            Assert.Equal(4.25m, _sut.Balance);
        }

        [Fact]
        public void Balance_ShouldApplyDiscountToProductsWithOutVolume()
        {
            discountProviderMock.Setup(x => x.GetDiscount(It.IsAny<decimal>())).Returns(20);
            priceProviderMock.Setup(x => x.GetPrice("B")).Returns(new Price { Name = "B", ItemPrice = 2m });

            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("A");
            _sut.Scan("B");
            _sut.Scan("B");

            Assert.Equal(7.45m, _sut.Balance);
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
