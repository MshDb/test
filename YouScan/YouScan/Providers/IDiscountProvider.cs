namespace YouScan.Providers
{
    public interface IDiscountProvider
    {
        decimal GetDiscount(decimal amount);
    }
}
