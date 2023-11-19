using Windows.Foundation.Collections;

namespace ProjectSBS.Services.Settings;

public class SettingsService : ISettingsService
{
    private readonly IPropertySet SettingsStorage = ApplicationData.Current.LocalSettings.Values;

    public void SetValue<T>(string key, T value)
    {
        if (!SettingsStorage.ContainsKey(key))
        {
            SettingsStorage.Add(key, value);
            return;
        }

        SettingsStorage[key] = value;
    }

    public T GetValue<T>(string key, T defaultValue = default)
    {
        if (SettingsStorage.TryGetValue(key, out object value))
        {
            try
            {
                return (T)value;
            }
            catch
            {
                // Corrupted storage, return default
                return defaultValue;
            }
        }

        return defaultValue;
    }

    private readonly string _defaultCurrencyId = "DefaultCurrency";
    public string DefaultCurrency
    {
        get => GetValue(_defaultCurrencyId, "EUR");
        set => SetValue(_defaultCurrencyId, value);
    }
}
