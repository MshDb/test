using System.Collections.Generic;
using System.Linq;

namespace YouScan.Providers
{
    public class PriceProvider : IPriceProvider
    {
        private Dictionary<string, Price> _pricing;

        public void SetPricing(IList<Price> pricing)
        {
            _pricing = pricing.ToDictionary(x => x.Name, x =>
            {
                x.Volumes ??= new Dictionary<int, decimal>();
                return x;
            });
        }

        public Price GetPrice(string name)
        {
            _pricing.TryGetValue(name, out var price);
            return price;
        }
    }
}
