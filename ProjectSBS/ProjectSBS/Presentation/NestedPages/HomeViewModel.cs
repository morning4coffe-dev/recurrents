using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Filtering;
using ProjectSBS.Services.User;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.NestedPages;

public partial class HomeViewModel : ViewModelBase
{
    private readonly IUserService _userService;
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;

    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private decimal _sum;

    [ObservableProperty]
    private Type? _itemDetails;

    [ObservableProperty]
    private string _welcomeMessage;

    [ObservableProperty]
    private bool _isPaneOpen;

    private ItemViewModel? _selectedItem;
    public ItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            IsPaneOpen = _selectedItem is { };

            if (_selectedItem == value)
            {
                return;
            }

            //Created only after user first requests opening item
            ItemDetails ??= typeof(ItemDetails);

            WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(value, (value?.Item is null)));

            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ItemViewModel> Items { get; } = new();

    public List<FilterCategory> Categories { get; }

    public FilterCategory SelectedCategory
    {
        get => _filterService.SelectedCategory;
        set
        {
            _filterService.SelectedCategory = value;

            WeakReferenceMessenger.Default.Send(new CategorySelectionChanged());

            OnPropertyChanged();

            _ = RefreshItems();
        }
    }

    //public ICommand Logout { get; }
    public ICommand AddNewCommand { get; }
    public ICommand DeleteCommand { get; }

    public HomeViewModel(
        IUserService userService,
        IItemService itemService,
        IItemFilterService filterService)
    {
        _userService = userService;
        _itemService = itemService;
        _filterService = filterService;

        AddNewCommand = new RelayCommand(AddNew);
        DeleteCommand = new AsyncRelayCommand(() => DeleteItem());

        Categories = filterService.Categories;

        var welcome = DateTime.Now.Hour switch
        {
            >= 5 and < 12 => "Good Morning",
            >= 12 and < 17 => "Good Afternoon",
            >= 17 and < 20 => "Good Evening",
            _ => "Good Night"
        };

        WelcomeMessage = welcome += ",";
    }

    public override async void Load()
    {
        User = await _userService.GetUser();

        await _itemService.InitializeAsync();
        var items = await RefreshItems();

        Sum = items.Sum(i => i.Item.Billing.BasePrice);

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif

        WeakReferenceMessenger.Default.Register<ItemUpdated>(this, (r, m) =>
        {
            if (m.Canceled)
            {
                SelectedItem = null;
                return;
            }

            ItemViewModel? item = null;

            if (SelectedItem is not null)
            {
                item = Items.FirstOrDefault(i => i.Item.Id == SelectedItem.Item?.Id);
            }

            if (m.ToSave)
            {
                if (item is null)
                {
                    _itemService.NewItem(m.Item.Item);
                }
                else
                {
                    _itemService.UpdateItem(item);
                }
            }

            SelectedItem = null;

            _ = RefreshItems();
        });

        WeakReferenceMessenger.Default.Register<ItemDeleted>(this, (r, m) =>
        {
            DeleteItem(m.Item);
        });

        WeakReferenceMessenger.Default.Register<CategorySelectionChanged>(this, (r, m) =>
        {
            OnPropertyChanged(nameof(SelectedCategory));
        });
    }

    public override void Unload()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
#endif
    }

    private async Task<IEnumerable<ItemViewModel>> RefreshItems()
    {
        var items = _itemService.GetItems(SelectedCategory.Selector);

        App.Dispatcher.TryEnqueue(() =>
        {
            Items.Clear();
            //TODO Dont clear the whole thing, just removem update and add changed
            Items.AddRange(items);
        });

        return items;
    }

    private void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    }

    private void AddNew()
    {
        SelectedItem = null;

        WeakReferenceMessenger.Default.Send(
            new ItemSelectionChanged(
                new ItemViewModel(new Item(null, String.Empty)), 
                true, 
                true)
            );
        IsPaneOpen = true;
    }

    public async Task DeleteItem(ItemViewModel? item = null)
    {
        _itemService.DeleteItem(item ?? SelectedItem);

        SelectedItem = null;
        await RefreshItems();

        return;
    }
}