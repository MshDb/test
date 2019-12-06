using System.Collections.Generic;
using System.Linq;
using YouScan.Model;

namespace YouScan.Providers
{
    public class DiscountProvider : IDiscountProvider
    {
        private IList<DiscountSet> _dicountList;

        public void SetDicountTable(IList<DiscountSet> dicountTable)
        {
            _dicountList = dicountTable;
            _dicountList.Add(new DiscountSet { StartingValue = decimal.MinValue, Discount = 0 });
        }

        public decimal GetDiscount(decimal amount)
        {
            return _dicountList.Where(x => x.StartingValue <= amount).OrderByDescending(x => x.StartingValue).FirstOrDefault().Discount;
        }
    }
}
