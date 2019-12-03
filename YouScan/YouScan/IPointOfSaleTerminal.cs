namespace YouScan
{
    public interface IPointOfSaleTerminal
    {
        decimal Balance { get; }

        void Scan(string name);

        //Охрана отмена!
        void Cancel(string name);

        public decimal Bill();
    }
}