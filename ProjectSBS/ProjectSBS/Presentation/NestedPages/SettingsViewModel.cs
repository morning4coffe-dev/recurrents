namespace ProjectSBS.Presentation.NestedPages;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ICurrencyCache _currencyCache;
    private readonly ISettingsService _settingsService;
    private readonly IUserService _userService;

    [ObservableProperty]
    private User? _user;

    public int SelectedCurrency
    {
        set => _settingsService.DefaultCurrency = Currencies[value];
        get
        {
            for (int i = 0; i < Currencies.Count; i++)
            {
                if (Currencies[i] == _settingsService.DefaultCurrency)
                {
                    return i;
                }

            }
            return 0;
        }
    }

    public ObservableCollection<string> Currencies { get; } = new();

    public readonly string TitleText;
    public SettingsViewModel(
    IStringLocalizer localizer,
    ICurrencyCache currencyCache,
    ISettingsService settingsService,
    IUserService userService)
    {
        _userService = userService;
        _currencyCache = currencyCache;
        _settingsService = settingsService;

        TitleText = localizer["Settings"];
    }

    public override async void Load()
    {
        User = await _userService.GetUser();

        var currency = await _currencyCache.GetCurrency(CancellationToken.None);

        if (currency?.Rates.Count == 0)
        {
            return;
        }

        //TODO sets the value on the open of the Settings
        Currencies.AddRange(currency?.Rates.Keys);
        Currencies.Add(currency?.BaseCurrency ?? "EUR");
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }
}
