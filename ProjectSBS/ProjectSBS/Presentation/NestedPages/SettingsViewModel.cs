namespace ProjectSBS.Presentation.NestedPages;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly ICurrencyCache _currencyCache;
    public readonly ISettingsService SettingsService;
    private readonly IUserService _userService;

    [ObservableProperty]
    private User? _user;

    public ObservableCollection<string> Currencies { get; } = new();

    public readonly string TitleText;
    public SettingsViewModel(
    IStringLocalizer localizer,
    IAuthenticationService authentication,
    ICurrencyCache currencyCache,
    ISettingsService settingsService,
    IUserService userService)
    {
        _userService = userService;
        _currencyCache = currencyCache;
        SettingsService = settingsService;
        _authentication = authentication;

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

        Currencies.Add(currency?.BaseCurrency ?? "EUR");
        Currencies.AddRange(currency?.Rates.Keys);
    }

    public override void Unload()
    {
        //throw new NotImplementedException();
    }
}
