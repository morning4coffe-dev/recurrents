using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.User;
using System.Collections.ObjectModel;
using Windows.System.Profile;
using ProjectSBS.Services.Items.Filtering;

namespace ProjectSBS.Presentation;

public partial class MainViewModel : ObservableObject
{
    private readonly IAuthenticationService _authentication;
    private readonly IUserService _userService;

    private readonly IItemService _itemService;

    private readonly IDispatcher _dispatch;
    private readonly INavigator _navigator;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private decimal _sum;

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

            if (IsMobile && value != null)
            {
                SentItem = value;
                _ = _navigator.NavigateViewModelAsync<ItemDetailsViewModel>(this);
            }

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

            _selectedCategory = value;

            _ = RefreshItems();
        }
    }

    private static ItemViewModel? _sentItem;
    public static ItemViewModel? SentItem
    {
        get => _sentItem;
        set
        {
            _sentItem = value;
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
        _dispatch = dispatch;
        _navigator = navigator;

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

        WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, (r, m) =>
        {
            ItemViewModel? item = null;

            if (SelectedItem is not null)
            {
                item = Items.FirstOrDefault(i => i.Item.Id == SelectedItem.Item?.Id);
            }

            if (item is null)
            {
                _itemService.NewItem(m.Item.Item);

                if (IsMobile)
                {
                    _navigator.GoBack(this);
                }
            }

            SelectedItem = null;
        });
    }

    private async Task InitializeAsync()
    {
        _selectedCategory = Categories[0];

        var user = await _userService.GetUser();

        await _dispatch.ExecuteAsync(() =>
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

        await _itemService.InitializeAsync();

        var items = await RefreshItems();

        await _dispatch.ExecuteAsync(() =>
        {
            Sum = items.Sum(i => i.Item.Billing.BasePrice);
        });
    }

    private async Task<IEnumerable<ItemViewModel>> RefreshItems()
    {
        var items = _itemService.GetItems(SelectedCategory.Selector);

        await _dispatch.ExecuteAsync(() =>
        {
            Items.Clear();
            Items.AddRange(items);
        });

        return items;
    }

    private async Task GoToSecondView()
    {
        //await _navigator.NavigateViewModelAsync<ItemDetailsViewModel>(this, data: new Entity(Name!));
    }

    public async Task DoLogout(CancellationToken token)
    {
        await _authentication.LogoutAsync(_dispatch, token);
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