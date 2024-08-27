namespace Recurrents.Presentation;

public partial class ItemDetailsViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    //private readonly ITagService _tagService;
    //private readonly IItemFilterService _filterService;
    //private readonly ICurrencyCache _currencyCache;
    //private readonly IDialogService _dialog;
    #endregion

    [ObservableProperty]
    private ItemViewModel? _selectedItem;

    [ObservableProperty]
    private string _itemName = "";

    [ObservableProperty]
    private bool _isEditing = false;

    public ObservableCollection<Tag> Tags { get; } = [];
    public ObservableCollection<string> Currencies { get; } = [];
    public ObservableCollection<string> FuturePayments { get; } = [];
    public List<KeyValuePair<Period, string>> PaymentPeriods { get; }

    public ItemDetailsViewModel(
        ItemViewModel? item,
        IStringLocalizer localizer
        )
    {
        _localizer = localizer;

        _selectedItem = item;

        PaymentPeriods = new Dictionary<Period, string>
        {
            { Period.Daily, _localizer["Daily"] },
            { Period.Weekly, _localizer["Weekly"] },
            { Period.Monthly, _localizer["Monthly"] },
            { Period.Quarterly, _localizer["Quarterly"] },
            { Period.Annually, _localizer["Annually"] }
        }.ToList();
    }

    public async void Load()
    {

    }

    [RelayCommand]
    private void EnableEditing()
    {
        //IsEditing = true;

        //// take a copy of the item
        //EditItem = SelectedItem?.Item with { };
        //ItemName = _localizer[string.IsNullOrEmpty(SelectedItem?.Item.Name) ? "NewItem" : "Edit"];
    }

    [RelayCommand]
    private async Task<bool> Close()
    {
        ContentDialogResult result = ContentDialogResult.Primary;

        //if (IsEditing)
        //{
        //    result = await _dialog.ShowAsync(
        //        _localizer["CloseDialogTitle"],
        //        _localizer["CloseDialogDescription"],
        //        _localizer["Ok"]);
        //}

        if (result == ContentDialogResult.Primary)
        {
            //WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, Canceled: true));
        }

        return result == ContentDialogResult.Primary;
    }

    [RelayCommand]
    private void Archive()
    {
        //if (SelectedItem is not { })
        //{
        //    return;
        //}

        //WeakReferenceMessenger.Default.Send(new ItemArchived(SelectedItem));
    }

    //private async void System_BackRequested(object? sender, BackRequestedEventArgs e)
    //{
    //    e.Handled = true;
    //    var close = await Close();

    //    if (close)
    //    {
    //        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    //    }
    //}
}
