namespace ProjectSBS.Services.Caching;

public sealed class CurrencyCache : ICurrencyCache
{
    private readonly IApiClient _api;
    private readonly ISerializer _serializer;
    private readonly ILogger _logger;

    public CurrencyCache(IApiClient api, ISerializer serializer, ILogger<CurrencyCache> logger)
    {
        _api = api;
        _serializer = serializer;
        _logger = logger;
    }

    private bool IsConnected
    {
        get
        {
            try
            {
                return NetworkInformation.GetInternetConnectionProfile().GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
            catch { return false; }
        }
    }

    public async ValueTask<Currency?> GetCurrency(CancellationToken token)
    {
        var currencyText = await GetCachedCurrency();
        if (!string.IsNullOrWhiteSpace(currencyText))
        {
            return _serializer.FromString<Currency>(currencyText);
        }

        if (!IsConnected)
        {
            _logger.LogWarning("App is offline and cannot connect to the API.");
            throw new Exception("No internet connection");
        }

        var response = await _api.GetCurrency(token);

        if (response.IsSuccessStatusCode && response.Content is not null)
        {
            var weather = response.Content;
            await Save(weather, token);
            return weather;
        }
        else if (response.Error is not null)
        {
            _logger.LogError(response.Error, "An error occurred while retrieving the latest Currencies.");
            throw response.Error;
        }
        else
        {
            return null;
        }
    }

    private async ValueTask<StorageFile> GetFile(CreationCollisionOption option) =>
        await ApplicationData.Current.TemporaryFolder.CreateFileAsync("currency.json", option);

    private async ValueTask<string?> GetCachedCurrency()
    {
        var file = await GetFile(CreationCollisionOption.OpenIfExists);
        var properties = await file.GetBasicPropertiesAsync();

        // Reuse latest cache file if offline
        // or if the file is less than 3 days old
        if (IsConnected || DateTimeOffset.Now.AddDays(-3) > properties.DateModified)
        {
            return null;
        }

        return await File.ReadAllTextAsync(file.Path);
    }

    private async ValueTask Save(Currency weather, CancellationToken token)
    {
        var weatherText = _serializer.ToString(weather);
        var file = await GetFile(CreationCollisionOption.ReplaceExisting);
        await File.WriteAllTextAsync(file.Path, weatherText, token);
    }
}
