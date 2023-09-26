using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.NestedPages;

public partial class HomeViewModel : ObservableObject
{
    private INavigator _navigator;
    private IDispatcher _dispatch;
    private IItemService _itemService;

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

    public ICommand EnableEditingCommand { get; }
    public ICommand CloseCommand { get; }

    public HomeViewModel(
        INavigator navigator,
        IDispatcher dispatch,
        IItemService itemService)
    {
        _navigator = navigator;
        _dispatch = dispatch;
        _itemService = itemService;

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif

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

        //InitializeAsync();
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
        var items = _itemService.GetItems(/*TODO Filtering SelectedCategory.Selector*/);

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
}