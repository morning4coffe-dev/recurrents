using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.User;
using System.Collections.ObjectModel;
using Windows.System.Profile;
using ProjectSBS.Services.Items.Filtering;
using ProjectSBS.Presentation.NestedPages;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;

    private readonly IItemService _itemService;

    public static IDispatcher Dispatch { get; private set; }
    private readonly INavigator _navigator;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private Type? _pageType;

    [ObservableProperty]
    private Type? _settingsPage;

    public bool IsMobile
    {
        get
        {
            var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            return deviceFamily.Contains("Mobile");
        }
    }


    public string? Title { get; }

    public ObservableCollection<ItemViewModel> Items { get; } = new();

    private ItemViewModel? _selectedItem;
    public ItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem == value)
            {
                return;
            }

            //Created only after user first requests opening item
            //ItemDetails ??= typeof(ItemDetails);

            WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(value));

            IsEditing = false;

            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public List<FilterCategory> Categories { get; }

    private FilterCategory _selectedCategory;
    public FilterCategory SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value)
            {
                return;
            }

            if (PageType != typeof(HomePage))
            {
                PageType = typeof(HomePage);
            }

            _selectedCategory = value;

            //_ = RefreshItems();
        }
    }

    public ICommand GoToSecond { get; }
    public ICommand Logout { get; }
    public ICommand AddNewCommand { get; }
    public ICommand SubmitChangesCommand { get; }
    public ICommand CloseDetailsCommand { get; }
    public ICommand EnableEditingCommand { get; }
    public ICommand DeleteItemCommand { get; }
    public ICommand ArchiveItemCommand { get; }

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
#if !__IOS__
        IAuthenticationService authentication,
#endif
        IUserService userService,
        IItemService itemService,
        IDispatcher dispatch,
        INavigator navigator,
        IItemFilterService filterService)
    {
#if !__IOS__
        _authentication = authentication;
#endif
        _userService = userService;
        _itemService = itemService;
        _navigator = navigator;
        Dispatch = dispatch;

        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";

        GoToSecond = new AsyncRelayCommand(GoToSecondView);
        Logout = new AsyncRelayCommand(DoLogout);
        AddNewCommand = new RelayCommand(AddNew);
        SubmitChangesCommand = new AsyncRelayCommand(SubmitChanges);
        CloseDetailsCommand = new AsyncRelayCommand(CloseDetails);
        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);
        ArchiveItemCommand = new AsyncRelayCommand(ArchiveItem);
        DeleteItemCommand = new AsyncRelayCommand(DeleteItem);

        Categories = filterService.Categories;

        Task.Run(InitializeAsync);
    }

    private async Task InitializeAsync()
    {
        _selectedCategory = Categories[0];

        var user = await _userService.GetUser();

        await Dispatch.ExecuteAsync(() =>
        {
            if (user is not null)
            {
                User = user;
                Name = User.Name;
            }
            else
            {
                //TODO enable login button
            }            
        });

        while ((Application.Current as App)!.Host == null)
        {
            //Wait till Host is created
            await Task.Delay(200);

            //TODO Add loading indicator
        }

        await Dispatch.ExecuteAsync(() =>
        {
            PageType = typeof(HomePage);
        });
    }

    private async Task GoToSecondView()
    {
        PageType = typeof(SettingsPage);
    }

    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(Dispatch, token);
        //TODO probably will have to clean the token too
    }

    private void AddNew()
    {
        SelectedItem = new ItemViewModel(null);

        IsEditing = true;
    }

    private async Task SubmitChanges()
    {
        SelectedItem = null;

        IsEditing = false;
    }

    private async Task EnableEditing()
    {
        IsEditing = true;
    }

    private async Task DeleteItem()
    {
        //TODO Find item by Id in database and delete it
        return;
    }

    private async Task ArchiveItem()
    {
        //TODO Find item by Id in database and delete it
        return;
    }

    private async Task CloseDetails()
    {
        //TODO Instead of IsEditing, check if the item is dirty
        if (IsEditing)
        {
            await _navigator.ShowMessageDialogAsync(this, title: "...", content: "Really?");
        }
        else
        {
            SelectedItem = null;
        }

        IsEditing = false;
    }
}