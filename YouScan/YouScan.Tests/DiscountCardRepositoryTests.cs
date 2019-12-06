using System;
using Xunit;
using YouScan.Repositories;

namespace YouScan.Tests
{
    public class DiscountCardRepositoryTests
    {
        public DiscountCardRepository _sut;
        private Guid _existingCustomer = Guid.NewGuid();

        public DiscountCardRepositoryTests()
        {            
            _sut = new DiscountCardRepository();
            _sut.Add(_existingCustomer, 1000);
        }

        [Fact]
        public void Add_ShouldAddNewCustomer()
        {
            var customerId = Guid.NewGuid();

            _sut.Add(customerId, 10);

            Assert.Equal(10, _sut.Get(customerId));
        }

        [Fact]
        public void Add_ShouldAddAmountToExistingCustomer()
        {
            _sut.Add(_existingCustomer, 500);

            Assert.Equal(1500, _sut.Get(_existingCustomer));
        }

        [Fact]
        public void Add_ShouldNotAddAmountToEmptyCustomer()
        {
            _sut.Add(Guid.Empty, 100);
            Assert.Equal(0, _sut.Get(Guid.Empty));
        }

        [Fact]
        public void Get_ExistingCustomer()
        {
            Assert.Equal(1000, _sut.Get(_existingCustomer));
        }

        [Fact]
        public void Get_NotExistingCustomer()
        {
            Assert.Equal(0, _sut.Get(Guid.NewGuid()));
        }
    }
}
