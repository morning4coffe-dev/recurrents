namespace Recurrents.Presentation;

public partial class ArchiveViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    private readonly IItemService _itemService;
    private readonly INavigator _navigation;
    private readonly IDispatcher _dispatcher;
    #endregion

    [ObservableProperty]
    public ItemViewModel? _selectedItem;

    public ObservableCollection<ItemViewModel> Items { get; } = [];

    public ArchiveViewModel(
        IItemService itemService,
        IStringLocalizer localizer,
        INavigator navigation,
        IDispatcher dispatcher)
    {
        _localizer = localizer;
        _itemService = itemService;
        _navigation = navigation;
        _dispatcher = dispatcher;

        RefreshItems();
    }


    private IEnumerable<ItemViewModel> RefreshItems(IEnumerable<ItemViewModel>? items = default)
    {
        var effectiveItems = items ?? _itemService.GetItems(item => !item.IsArchived)
                                                   .OrderBy(i => i.PaymentDate);

        var filteredAndSortedItems = effectiveItems.Where(item => !item.IsArchived)
                                                   .OrderBy(i => i.PaymentDate)
                                                   .ToList();

        _dispatcher.TryEnqueue(() =>
        {
            Items.Clear();
            Items.AddRange(filteredAndSortedItems);
        });

        return filteredAndSortedItems;
    }

    //    [RelayCommand]
    //    public async Task Archive(ItemViewModel? item = null)
    //    {
    //        if (item is not { } && SelectedItem is not { })
    //        {
    //            return;
    //        }

    //        ContentDialogResult result;

    //        if (!(item ?? SelectedItem).IsArchived)
    //        {
    //            result = await _dialog.ShowAsync(
    //                _localizer["ArchiveDialogTitle"],
    //                _localizer["ArchiveDialogDescription"],
    //                _localizer["ArchiveVerb"]);
    //        }
    //        else
    //        {
    //            result = ContentDialogResult.Primary;
    //        }

    //        if (result == ContentDialogResult.Primary)
    //        {
    //            _itemService.ArchiveItem(item ?? SelectedItem);

    //            AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Archived",
    //                (item ?? SelectedItem).IsArchived.ToString());

    //            SelectedItem = null;
    //            RefreshItems();
    //        }
    //    }

    //    [RelayCommand]
    //    public async Task Delete(ItemViewModel? item = null)
    //    {
    //        if (item is not { } && SelectedItem is not { })
    //        {
    //            return;
    //        }

    //        var result = await _dialog.ShowAsync(
    //            _localizer["DeleteDialogTitle"],
    //            _localizer["DeleteDialogDescription"],
    //            _localizer["Delete"]);

    //        if (result == ContentDialogResult.Primary)
    //        {
    //            _itemService.DeleteItem(item ?? SelectedItem);

    //            AnalyticsService.TrackEvent(AnalyticsService.ItemEvent, "Deleted", "True");

    //            SelectedItem = null;
    //            RefreshItems();
    //        }
    //    }

    [RelayCommand]
    private void OpenSettings()
        => _navigation.NavigateViewModelAsync<SettingsViewModel>(this);
}
