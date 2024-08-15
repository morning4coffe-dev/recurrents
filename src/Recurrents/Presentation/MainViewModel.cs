using Uno.Extensions.Navigation.UI;

namespace Recurrents.Presentation;

public partial class MainViewModel : ObservableObject
{
    private readonly IStringLocalizer _localizer;
    private readonly INavigator _navigation;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private bool _indicateLoading;

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigation)
    {
        _localizer = localizer;
        _navigation = navigation;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSecond = new AsyncRelayCommand(GoToSecondView);

        Categories =
        [
            //new(0, _localizer["Home"], "\uE80F", typeof(HomePage)),
            //new(1, _localizer["Items"], "\uF0B2", typeof(HomePage)),
            //new(2, _localizer["Archive"], "\uE7B8", typeof(ArchivePage)),
            //new(5, _localizer["Settings"], "\uE713", typeof(SettingsPage)),
            new(0, _localizer["Home"], "\uE80F", typeof(SecondPage)),
            new(1, _localizer["Items"], "\uF0B2", typeof(SecondPage)),
            new(2, _localizer["Archive"], "\uE7B8", typeof(SecondPage)),
            new(5, _localizer["Settings"], "\uE713", typeof(SecondPage)),
        ];
    }
    public string? Title { get; }

    public ICommand GoToSecond { get; }

    public IEnumerable<NavigationCategory> Categories { get; }

    private NavigationCategory _selectedCategory;
    public NavigationCategory SelectedCategory
    {
        set
        {
            if (_selectedCategory != value)
            {
                _selectedCategory = value;

                _navigation.NavigateViewAsync(this, _selectedCategory.Page);
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        get => _selectedCategory;
    }

    private async Task GoToSecondView()
    {
        /*await _navigator.NavigateViewModelAsync<ThirdViewModel>(this, data: new Entity(Name!));*/
    }

}
