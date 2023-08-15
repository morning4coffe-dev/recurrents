using Windows.Foundation.Collections;

namespace ProjectSBS.Services.Settings;

internal class SettingsService
{
    private readonly IPropertySet SettingsStorage = ApplicationData.Current.LocalSettings.Values;

    public void SetValue<T>(string key, T value)
    {
        if (!SettingsStorage.ContainsKey(key))
            SettingsStorage.Add(key, value);
        else
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
            }
        }

        return defaultValue;
    }
}
