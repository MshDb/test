using System;

namespace YouScan
{
    public interface IPointOfSaleTerminal
    {
        decimal Balance { get; }

        void Scan(string name);

        //Охрана отмена!
        void Cancel(string name);

        void ScanDiscountCard(Guid? customerId = null);

        decimal Bill();
    }
}