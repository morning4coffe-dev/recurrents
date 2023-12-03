using Windows.UI.Core;

namespace ProjectSBS.Business.ViewModels;

public partial class ItemDetailsViewModel : ViewModelBase
{
    private readonly IStringLocalizer _localizer;
    private readonly ITagService _tagService;
    private readonly ICurrencyCache _currencyCache;

    [ObservableProperty]
    private ItemViewModel? _selectedItem;

    [ObservableProperty]
    private Item? _editItem;

    [ObservableProperty]
    private string _itemName = "";

    [ObservableProperty]
    private bool _isEditing = false;

    public ObservableCollection<Tag> Tags { get; } = [];
    public ObservableCollection<string> Currencies { get; } = [];
    public ObservableCollection<string> FuturePayments { get; } = [];

    public string SaveText { get; }
    public string EditText { get; }

    public ICommand EnableEditingCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ArchiveCommand { get; }

    public ItemDetailsViewModel(
        IStringLocalizer localizer,
        ITagService tagService,
        ICurrencyCache currencyCache)
    {
        _localizer = localizer;
        _tagService = tagService;
        _currencyCache = currencyCache;

        EnableEditingCommand = new RelayCommand(() => EnableEditing());
        CloseCommand = new AsyncRelayCommand(Close);
        SaveCommand = new RelayCommand(Save);
        ArchiveCommand = new RelayCommand(Archive);

        SaveText = localizer["Save"];
        EditText = localizer["Edit"];
    }

    public async override void Load()
    {
        Tags.AddRange(_tagService.Tags.ToObservableCollection());

        WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, (r, m) =>
        {
#if HAS_UNO
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif
            if (m.SelectedItem is { } item)
            {
                ItemName = item.Item?.Name ?? "";

                SelectedItem = item;
                OnPropertyChanged(nameof(SelectedItem));

                EnableEditing(m.IsEdit);

                var localizedDateStrings
                    = item.GetFuturePayments()
                          .Select(date => date.ToString(CultureInfo.CurrentCulture))
                          .ToList();

                FuturePayments.Clear();
                FuturePayments.AddRange(localizedDateStrings);
            }
        });

        var currency = await _currencyCache.GetCurrency(CancellationToken.None);

        if (currency?.Rates.Count > 0)
        {
            Currencies.Add(currency.BaseCurrency);
            Currencies.AddRange(currency.Rates.Keys);
        }
    }

    public override void Unload()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }


    private void EnableEditing(bool isTrue = true)
    {
        IsEditing = isTrue;

        if (isTrue)
        {
            // take a copy of the item
            EditItem = SelectedItem?.Item with { };
            ItemName = _localizer[string.IsNullOrEmpty(SelectedItem?.Item.Name) ? "NewItem" : "Edit"];
        }
    }

    private async Task<bool> Close()
    {
        if (IsEditing)
        {
            //TODO Switch to new Dialog on Dispatcher
            //await MainViewModel.Navigator.ShowMessageDialogAsync(
            //    this,
            //    title: _localizer?["Leave"] ?? "Really wanna leave?",
            //    content: _localizer?["Dialog_Ok"] ?? "Really?",
            //    buttons: new[]
            //    {
            //        new DialogAction(
            //            Label: _localizer?["Ok"] ?? "Ok",
            //            Action: () => { IsEditing = false; }),
            //        new DialogAction(
            //            Label: _localizer?["Cancel"] ?? "Cancel")
            //    });

            //return false;
        }

        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, Canceled: true));

        return true;
    }

    private void Save()
    {
        if (EditItem is not { } item || SelectedItem is not { })
        {
            return;
        }

        SelectedItem.Item = item;
        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, ToSave: true));
    }

    private void Archive()
    {
        WeakReferenceMessenger.Default.Send(new ItemArchived(SelectedItem));
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
