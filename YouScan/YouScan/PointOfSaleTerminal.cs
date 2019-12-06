using System;
using System.Collections.Generic;
using System.Linq;
using YouScan.Providers;
using YouScan.Repositories;

namespace YouScan
{
    public class PointOfSaleTerminal : IPointOfSaleTerminal
    {
        private readonly IPriceProvider _priceProvider;
        private readonly IDiscountCardRepository _discountCardRepository;
        private readonly IDiscountProvider _discountProvider;
        private Dictionary<string, ShoppingCartItem> _shoppingCart;
        private Guid _customerId = Guid.Empty;

        public PointOfSaleTerminal(IPriceProvider priceProvider, IDiscountCardRepository discountCardRepository, IDiscountProvider discountProvider)
        {
            _priceProvider = priceProvider;
            _discountCardRepository = discountCardRepository;
            _discountProvider = discountProvider;
            _shoppingCart = new Dictionary<string, ShoppingCartItem>();
        }

        private class ShoppingCartItem
        {
            public int Count { get; set; }
            public Price Price { get; set; }
        }

        public decimal Balance {
            get
            {
                var currentAmount = _discountCardRepository.Get(_customerId);
                var discountPercent = _discountProvider.GetDiscount(currentAmount);
                return _shoppingCart.Sum(cartItem =>
                {
                    var item = cartItem.Value;
                    var maxVol = item.Price.Volumes?.Where(x => x.Key <= item.Count).OrderByDescending(x => x.Key).FirstOrDefault();
                    if (maxVol.GetValueOrDefault().Equals(default(KeyValuePair<int,decimal>)))
                    {
                        return item.Count * item.Price.ItemPrice * (100 - discountPercent) * 0.01m;
                    }

                    var volume = maxVol.Value;
                    return (item.Count - volume.Key) * item.Price.ItemPrice + volume.Value;
                });
            }
        }

        public void Scan(string name)
        {
            var price = _priceProvider.GetPrice(name);

            if (price != null)
            {
                if (_shoppingCart.TryGetValue(name, out var _))
                {
                    _shoppingCart[name].Count++;
                }
                else
                {
                    _shoppingCart.Add(name, new ShoppingCartItem { Price = price, Count = 1 });
                }
            }
        }

        public void Cancel(string name)
        {
            if (_shoppingCart.TryGetValue(name, out var _))
            {
                _shoppingCart[name].Count = _shoppingCart[name].Count > 0 ? _shoppingCart[name].Count - 1 : 0;
            }
            else
            {
                Console.WriteLine("Item is not in cart");
            }
        }

        public void ScanDiscountCard(Guid? customerId=null)
        {
            _customerId = customerId ?? Guid.NewGuid();
        }

        public decimal Bill()
        {
            var bal = Balance;
            _discountCardRepository.Add(_customerId, bal);

            _customerId = Guid.Empty;
            _shoppingCart = new Dictionary<string, ShoppingCartItem>();
            return bal;
        }
    }
}
