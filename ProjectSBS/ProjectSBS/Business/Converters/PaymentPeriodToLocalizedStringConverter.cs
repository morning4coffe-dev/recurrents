using Microsoft.UI.Xaml.Data;

namespace ProjectSBS.Business.Converters;

public class PaymentPeriodToLocalizedStringConverter : IValueConverter
{
    private readonly IStringLocalizer _localizer;

    public PaymentPeriodToLocalizedStringConverter()
    {
        _localizer = App.Services!.GetRequiredService<IStringLocalizer>();
    }

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Period period)
        {
            return _localizer[period.ToString()].Value;
        }

        return value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
