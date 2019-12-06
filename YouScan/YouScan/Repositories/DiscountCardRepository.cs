using System;
using System.Collections.Generic;

namespace YouScan.Repositories
{
    public class DiscountCardRepository : IDiscountCardRepository
    {
        private readonly Dictionary<Guid, decimal> _discountCardRepository;

        public DiscountCardRepository()
        {
            _discountCardRepository = new Dictionary<Guid, decimal>();
        }

        public void Add(Guid customerId, decimal amount)
        {
            if (customerId == Guid.Empty)
            {
                return;
            }

            if(_discountCardRepository.TryGetValue(customerId, out var _))
            {
                _discountCardRepository[customerId] += amount;
            }
            else
            {
                _discountCardRepository.Add(customerId, amount);
            }
        }

        public decimal Get(Guid customerId)
        {
            if(_discountCardRepository.TryGetValue(customerId, out var amount))
            {
                return amount;
            }

            return 0;
        }
    }
}
