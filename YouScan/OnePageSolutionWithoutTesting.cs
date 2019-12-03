using System;
using System.Collections.Generic;
using System.Linq;

namespace YouScan
{
	/*
    |A| $1.25 each or 3 for $3.00 
    |B| $4.25                     
    |C| $1.00 or $5 for a six pack
    |D| $0.75
    Scan these items in this order: ABCDABA; Verify the total price is $13.25.
    Scan these items in this order: CCCCCCC; Verify the total price is $6.00.
    Scan these items in this order: ABCD; Verify the total price is $7.25
    */
	class Program
	{
		private static IPointOfSaleTerminal _terminal;

		static void Main()
		{
			Console.WriteLine("Hello YouScan!");

			var priceProvider = new PriceProvider();

			priceProvider.SetPricing(
				new List<Price>
				{
					new Price{ Name="A", ItemPrice=1.25m, Volumes = new Dictionary<int, decimal>{{3,3}} },
					new Price{ Name="B", ItemPrice=4.25m },
					new Price{ Name="C", ItemPrice=1, Volumes = new Dictionary<int, decimal>{{6,5}} },
					new Price{ Name="D", ItemPrice=0.75m }
				});

			_terminal = new PointOfSaleTerminal(priceProvider);

			Run("ABCDABA");
			Run("CCCCCCC");
			Run("ABCD");
		}

		static void Run(string input)
		{
			input.ToList().ForEach(x => _terminal.Scan(x.ToString()));
			Console.WriteLine($"Total for {input}: {_terminal.Bill()}");
		}
	}

	public interface IPointOfSaleTerminal
	{
		decimal Balance { get; }

		void Scan(string name);

		//Охрана отмена!
		void Cancel(string name);

		public decimal Bill();
	}

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

	public interface IPriceProvider
	{
		Price GetPrice(string name);
	}

	public class PriceProvider : IPriceProvider
	{
		private Dictionary<string, Price> _pricing;

		public void SetPricing(IList<Price> pricing)
		{
			_pricing = pricing.ToDictionary(x => x.Name, x => x);
		}

		public Price GetPrice(string name)
		{
			_pricing.TryGetValue(name, out var price);
			return price;
		}
	}

	public class Price
	{
		public string Name { get; set; }
		public decimal ItemPrice { get; set; }
		public Dictionary<int, decimal> Volumes { get; set; }
	}
}