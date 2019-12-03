namespace YouScan.Providers
{
    public interface IPriceProvider
    {
        Price GetPrice(string name);
    }
}
