using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Filtering;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.NestedPages;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigator _navigator;
    private readonly IDispatcher _dispatch;
    private readonly IItemService _itemService;
    private readonly IItemFilterService _filterService;

    [ObservableProperty]
    private Type? _itemDetails;

    [ObservableProperty]
    private decimal _sum;

    [ObservableProperty]
    private bool _isEditing = false;

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

            WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(value));

            IsEditing = false;

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

    public HomeViewModel(
        INavigator navigator,
        IDispatcher dispatch,
        IItemService itemService,
        IItemFilterService filterService)
    {
        _navigator = navigator;
        _dispatch = dispatch;
        _itemService = itemService;
        _filterService = filterService;

        AddNewCommand = new RelayCommand(AddNew);

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