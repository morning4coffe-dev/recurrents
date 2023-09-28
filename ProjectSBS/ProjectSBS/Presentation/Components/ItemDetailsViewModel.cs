using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Tags;
using System.Collections.ObjectModel;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.Components;

public partial class ItemDetailsViewModel : ObservableObject
{
    private INavigator _navigator;
    private IDispatcher _dispatch;

    [ObservableProperty]
    private ItemViewModel? _selectedItem;

    [ObservableProperty]
    private string _itemName = "";

    [ObservableProperty]
    private bool _isEditing = false;

    public ObservableCollection<Tag> Tags { get; }

    [ObservableProperty]
    public Dictionary<string, double> _currencies = new();

    public string SaveText { get; }
    public string EditText { get; }

    public ObservableCollection<ItemViewModel> Items { get; } = new();

    public ICommand EnableEditingCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public ItemDetailsViewModel(
        INavigator navigator,
        IDispatcher dispatch,
        IStringLocalizer localizer,
        ITagService tagService,
        ICurrencyCache currencyCache,
        IItemService itemService)
    {
        _navigator = navigator;
        _dispatch = dispatch;

        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);
        CloseCommand = new AsyncRelayCommand(Close);
        SaveCommand = new AsyncRelayCommand(Save);

        SaveText = localizer["Save"];
        EditText = localizer["Edit"];

        Tags = tagService.Tags.ToObservableCollection();

        InitializeCurrency(currencyCache);

        ItemName = SelectedItem?.Item?.Name ?? "New Item";
        
        WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, (r, m) =>
        {
#if HAS_UNO
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif
            if (m.SelectedItem is { } item)
            {
                SelectedItem = item;

                //TODO get logs Items = item.
            }
        });
    }

    private async void InitializeCurrency(ICurrencyCache currencyCache)
    {
        var currency = await currencyCache.GetCurrency(CancellationToken.None);

        if (currency?.Rates.Count == 0)
        {
            return;
        }

        await MainViewModel.Dispatch.ExecuteAsync(() =>
        {
            Currencies.AddRange(currency.Rates);
        });
    }


    private async Task EnableEditing()
    {
        IsEditing = true;
    }

    private async Task Close()
    {
        //await _navigator.ShowMessageDialogAsync(this, title: "...", content: "Really?");
        //TODO Ask to save?

        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem));
    }

    private async Task Save()
    {
        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem));
    }

    private void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        _ = Close();
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    }
}