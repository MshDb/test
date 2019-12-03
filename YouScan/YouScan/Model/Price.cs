using System.Collections.Generic;

namespace YouScan
{
    public class Price
    {
        public string Name { get; set; }
        public decimal ItemPrice { get; set; }
        public Dictionary<int, decimal> Volumes { get; set; }
    }
}
