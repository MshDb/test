using YouScan.Providers;
using Xunit;
using System.Collections.Generic;

namespace YouScan.Tests
{
    public class PriceProviderTest
    {
        private PriceProvider _sut;
        public PriceProviderTest()
        {
            _sut = new PriceProvider();
            _sut.SetPricing(
                new List<Price>
                {
                    new Price{ Name="A", ItemPrice=1.25m, Volumes = new Dictionary<int, decimal>{{3,3}} },
                    new Price{ Name="B", ItemPrice=4.25m }
                }
            );
        }

        [Fact]
        public void GetPrice_ReturnsRightValues()
        {
            var result = _sut.GetPrice("A");

            Assert.Equal("A", result.Name);
            Assert.Equal(1.25m, result.ItemPrice);
            Assert.Equal(3, result.Volumes[3]);
        }

        [Fact]
        public void GetPrice_ReturnsFullItem()
        {
            var result = _sut.GetPrice("B");

            Assert.Equal("B", result.Name);
            Assert.Equal(4.25m, result.ItemPrice);
            Assert.NotNull(result.Volumes);
        }

        [Fact]
        public void GetPrice_ReturnsNullForUnknown()
        {
            var result = _sut.GetPrice("ASD");

            Assert.Null(result);
        }
    }
}
