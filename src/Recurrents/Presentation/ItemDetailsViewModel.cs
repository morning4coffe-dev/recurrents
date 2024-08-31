using System.Globalization;

namespace Recurrents.Presentation;

public partial class ItemDetailsViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigator;
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
        IStringLocalizer localizer,
        IItemService itemService,
        INavigator navigator)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigator = navigator;

        _selectedItem = item;

        PaymentPeriods = new Dictionary<Period, string>
        {
            { Period.Daily, _localizer["Daily"] },
            { Period.Weekly, _localizer["Weekly"] },
            { Period.Monthly, _localizer["Monthly"] },
            { Period.Quarterly, _localizer["Quarterly"] },
            { Period.Annually, _localizer["Annually"] }
        }.ToList();

        // _itemService.OnItemsChanged += OnItemsChanged; for after navigation from ItemEditViewModel

        var localizedDateStrings = item.GetFuturePayments()
                  .Select(date => date.ToString(CultureInfo.CurrentCulture))
                  .ToList();

        FuturePayments.Clear();
        FuturePayments.AddRange(localizedDateStrings);
    }

    public async void Load()
    {

    }

    [RelayCommand]
    private async Task Archive()
    {
        if (SelectedItem is not { })
        {
            throw new InvalidOperationException("No item selected!");
        }

        _itemService.ArchiveItem(SelectedItem);
        await _navigator.NavigateBackAsync(this);
    }
}
