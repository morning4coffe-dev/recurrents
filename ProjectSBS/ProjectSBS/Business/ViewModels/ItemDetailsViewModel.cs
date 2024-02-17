using Windows.UI.Core;

namespace ProjectSBS.Business.ViewModels;

public partial class ItemDetailsViewModel : ViewModelBase
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly ITagService _tagService;
    private readonly ICurrencyCache _currencyCache;
    private readonly IDialogService _dialog;
    #endregion

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
    public ObservableCollection<string> PaymentMethods { get; }

    public ICommand EnableEditingCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ArchiveCommand { get; }

    public ItemDetailsViewModel(
        IStringLocalizer localizer,
        ITagService tagService,
        ICurrencyCache currencyCache,
        IDialogService dialog)
    {
        _localizer = localizer;
        _tagService = tagService;
        _currencyCache = currencyCache;
        _dialog = dialog;

        EnableEditingCommand = new RelayCommand(() => EnableEditing());
        CloseCommand = new AsyncRelayCommand(Close);
        SaveCommand = new RelayCommand(Save);
        ArchiveCommand = new RelayCommand(Archive);

        PaymentMethods =
        [
            _localizer["CreditCard"],
            _localizer["DebitCard"],
            _localizer["DigitalWallet"],
            _localizer["BankTransfer"],
            _localizer["Cryptocurrency"],
            _localizer["Invoice"],
            _localizer["Cash"]
        ];
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
        ContentDialogResult result = ContentDialogResult.Primary;

        if (IsEditing)
        {
            result = await _dialog.ShowAsync(
                _localizer["CloseDialogTitle"],
                _localizer["CloseDialogDescription"],
                _localizer["Ok"]);
        }

        if (result == ContentDialogResult.Primary)
        {
            WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, Canceled: true));
        }

        return result == ContentDialogResult.Primary;
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
        if (SelectedItem is not { })
        {
            return;
        }

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
