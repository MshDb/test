using System;
using System.Collections.Generic;
using System.Linq;
using YouScan.Providers;

namespace YouScan
{
    public class PointOfSaleTerminal : IPointOfSaleTerminal
    {
        private readonly IPriceProvider _priceProvider;
        private Dictionary<string, ShoppingCartItem> _shoppingCart;

        public PointOfSaleTerminal(IPriceProvider priceProvider)
        {
            _priceProvider = priceProvider;
            _shoppingCart = new Dictionary<string, ShoppingCartItem>();
        }

        private class ShoppingCartItem
        {
            public int Count { get; set; }
            public Price Price { get; set; }
        }

        public decimal Balance =>
            _shoppingCart.Sum(cartItem =>
            {
                var item = cartItem.Value;
                var maxVol = item.Price.Volumes?.Where(x => x.Key <= item.Count).OrderByDescending(x => x.Key).FirstOrDefault() ?? new KeyValuePair<int, decimal>(0, 0);
                return (item.Count - maxVol.Key) * item.Price.ItemPrice + maxVol.Value;
            });

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

        public decimal Bill()
        {
            var bal = Balance;
            _shoppingCart = new Dictionary<string, ShoppingCartItem>();
            return bal;
        }
    }
}
