using System;
using System.Collections.Generic;
using System.Linq;
using YouScan.Model;
using YouScan.Providers;
using YouScan.Repositories;

namespace YouScan
{
    /*
    |A| $125 each or 3 for $300 
    |B| $425                     
    |C| $100 or $500 for a six pack
    |D| $75
    Scan these items in this order: ABCDABA; Verify the total price is $1325.
    Scan these items in this order: CCCCCCC; Verify the total price is $600.
    Scan these items in this order: ABCD; Verify the total price is $717.75 as we applied 1% discount
    */

    /*
    Introduce discount card:

    * the discount card is used only for products where no other discount applies
    * amount of sale (without discounts) is added to the card at the end of the sale
    * discount percent depends on the total amount of money accumulated on the discount card (total sum spent in our shop)

    Discount percent:
    Amount  	%
    1000-1999	1%
    2000-4999	3%
    5000-9999	5%
    over 9999	7%
    */

    class Program
    {
        private static IPointOfSaleTerminal _terminal;
        private static Guid _customerId = Guid.NewGuid();

        static void Main()
        {
            Console.WriteLine("Hello YouScan!");

            var priceProvider = new PriceProvider();
            priceProvider.SetPricing(
                new List<Price>
                {
                    new Price{ Name="A", ItemPrice=125m, Volumes = new Dictionary<int, decimal>{{3,300}} },
                    new Price{ Name="B", ItemPrice=425m },
                    new Price{ Name="C", ItemPrice=100, Volumes = new Dictionary<int, decimal>{{6,500}} },
                    new Price{ Name="D", ItemPrice=75m }
                });

            var discountProvider = new DiscountProvider();
            discountProvider.SetDicountTable(
                new List<DiscountSet>
                {
                    new DiscountSet{StartingValue=1000, Discount=1},
                    new DiscountSet{StartingValue=2000, Discount=3},
                    new DiscountSet{StartingValue=5000, Discount=5},
                    new DiscountSet{StartingValue=9999, Discount=7}
                });


            _terminal = new PointOfSaleTerminal(priceProvider, new DiscountCardRepository(), discountProvider);

            Run("ABCDABA");
            Run("CCCCCCC");
            Run("ABCD");
        }

        static void Run(string input)
        {
            input.ToList().ForEach(x => _terminal.Scan(x.ToString()));

            _terminal.ScanDiscountCard(_customerId);

            Console.WriteLine($"Total for {input}: {_terminal.Bill()}");
        }
    }
}