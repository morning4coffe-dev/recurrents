using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Filtering;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.NestedPages;

public partial class HomeViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;

    [ObservableProperty]
    private Type? _itemDetails;

    [ObservableProperty]
    private decimal _sum;

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

    public ICommand Logout { get; }
    public ICommand AddNewCommand { get; }
    public ICommand DeleteCommand { get; }

    public HomeViewModel(
        IItemService itemService,
        IItemFilterService filterService)
    {
        _itemService = itemService;
        _filterService = filterService;

        AddNewCommand = new RelayCommand(AddNew);
        DeleteCommand = new AsyncRelayCommand(() => DeleteItem());

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif

        Categories = filterService.Categories;

        Task.Run(InitializeAsync);

        WeakReferenceMessenger.Default.Register<ItemUpdated>(this, (r, m) =>
        {
            ItemViewModel? item = null;

            if (SelectedItem is not null)
            {
                item = Items.FirstOrDefault(i => i.Item.Id == SelectedItem.Item?.Id);
            }

            if (item is null)
            {
                _itemService.NewItem(m.Item.Item);
            }

            SelectedItem = null;
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

    private async Task InitializeAsync()
    {
        await _itemService.InitializeAsync();

        var items = await RefreshItems();

        await MainViewModel.Dispatch.ExecuteAsync(() =>
        {
            Sum = items.Sum(i => i.Item.Billing.BasePrice);
        });
    }

    private async Task<IEnumerable<ItemViewModel>> RefreshItems()
    {
        var items = _itemService.GetItems(SelectedCategory.Selector);

        await MainViewModel.Dispatch.ExecuteAsync(() =>
        {
            Items.Clear();
            //TODO Dont clear the whole thing, just removem update and add changed
            Items.AddRange(items);
        });

        foreach (var item in items)
        {
            Items.Add(item);
        }

        return items;
    }

    private void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    }

    private void AddNew()
    {
        SelectedItem = new ItemViewModel(null);
    }

    private async Task DeleteItem(ItemViewModel? item = null)
    {
        _itemService.DeleteItem(item ?? SelectedItem);

        await RefreshItems();

        return;
    }
}