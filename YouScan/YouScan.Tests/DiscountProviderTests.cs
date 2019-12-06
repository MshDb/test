using System.Collections.Generic;
using Xunit;
using YouScan.Model;
using YouScan.Providers;

namespace YouScan.Tests
{
    public class DiscountProviderTests
    {
        public DiscountProvider _sut;

        public DiscountProviderTests()
        {
            _sut = new DiscountProvider();
            _sut.SetDicountTable(
                new List<DiscountSet>
                {
                    new DiscountSet{StartingValue=1000, Discount=1},
                    new DiscountSet{StartingValue=2000, Discount=3},
                    new DiscountSet{StartingValue=5000, Discount=5},
                    new DiscountSet{StartingValue=9999, Discount=7}
                });
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, 0)]
        [InlineData(1000, 1)]
        [InlineData(10000, 7)]
        public void Get_ShouldReturnRightDiscount(decimal amount, int discount)
        {
            Assert.Equal(discount, _sut.GetDiscount(amount));
        }
    }
}
