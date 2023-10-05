using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using ProjectSBS.Services.Items;
using ProjectSBS.Services.Items.Tags;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.Components;

public partial class ItemDetailsViewModel : ObservableObject
{
    private readonly INavigator _navigator;
    private readonly IDispatcher _dispatch;

    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;

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
    public ObservableCollection<string> FuturePayments { get; } = new();

    public ICommand EnableEditingCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

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
        _localizer = localizer;
        _itemService = itemService;

        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);
        CloseCommand = new AsyncRelayCommand(Close);
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(DeleteItem);

        SaveText = localizer["Save"];
        EditText = localizer["Edit"];

        Tags = tagService.Tags.ToObservableCollection();

        InitializeCurrency(currencyCache);

        ItemName = SelectedItem?.Item?.Name ?? "New Item";

        //TODO Doesn't unregister when switching pages and registers again
        WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, (r, m) =>
        {
#if HAS_UNO
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif
            if (m.SelectedItem is { } item)
            {
                MainViewModel.Dispatch.ExecuteAsync(() =>
                {
                    SelectedItem = item;
                });

                IsEditing = m.IsEdit;

                FuturePayments.Clear();

                var localizedDateStrings 
                    = item.GetFuturePayments()
                          .Select(date => date.ToString(CultureInfo.CurrentCulture))
                          .ToList();

                FuturePayments.AddRange(localizedDateStrings);
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

    private async Task<bool> Close()
    {
        if (IsEditing)
        {
            await MainViewModel.Navigator.ShowMessageDialogAsync(
                this,
                title: _localizer?["Leave"] ?? "Really wanna leave?",
                content: _localizer?["Dialog_Ok"] ?? "Really?",
                buttons: new[]
                {
                    new DialogAction(
                        Label: _localizer?["Ok"] ?? "Ok", 
                        Action: () => { IsEditing = false; }),
                    new DialogAction(
                        Label: _localizer?["Cancel"] ?? "Cancel")
                });

            return false;
        }

        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem));

        return true;
    }

    private async Task Save()
    {
        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem));
    }

    private async Task DeleteItem()
    {
        WeakReferenceMessenger.Default.Send(new ItemDeleted(SelectedItem));
    }

    private async void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        var close = await Close();

        if (close)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
        }
    }
}