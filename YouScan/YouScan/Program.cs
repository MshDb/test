using System;
using System.Collections.Generic;
using System.Linq;
using YouScan.Providers;

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
}