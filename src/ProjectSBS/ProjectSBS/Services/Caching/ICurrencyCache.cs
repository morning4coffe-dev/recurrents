namespace ProjectSBS.Services.Caching;

public interface ICurrencyCache
{
    ValueTask<Currency?> GetCurrency(CancellationToken token);
}