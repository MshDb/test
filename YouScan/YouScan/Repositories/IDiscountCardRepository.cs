using System;

namespace YouScan.Repositories
{
    public interface IDiscountCardRepository
    {
        public void Add(Guid customerId, decimal amount);

        public decimal Get(Guid customerId);
    }
}
