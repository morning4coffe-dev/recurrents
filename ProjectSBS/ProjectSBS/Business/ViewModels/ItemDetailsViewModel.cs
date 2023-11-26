using Windows.UI.Core;

namespace ProjectSBS.Business.ViewModels;

public partial class ItemDetailsViewModel : ObservableObject
{
    private readonly IStringLocalizer _localizer;

    [ObservableProperty]
    private ItemViewModel? _selectedItem;

    [ObservableProperty]
    private string _itemName = "";

    [ObservableProperty]
    private bool _isEditing = false;
    private bool _isNew = false;

    public ObservableCollection<Tag> Tags { get; }
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

        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);
        CloseCommand = new AsyncRelayCommand(Close);
        SaveCommand = new AsyncRelayCommand(Save);
        ArchiveCommand = new AsyncRelayCommand(Archive);

        SaveText = localizer["Save"];
        EditText = localizer["Edit"];

        Tags = tagService.Tags.ToObservableCollection();

        InitializeCurrency(currencyCache);

        //TODO Doesn't unregister when switching pages and registers again
        WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, async (r, m) =>
        {
#if HAS_UNO
            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif
            if (m.SelectedItem is { } item)
            {
                _isNew = m.IsNew;

                App.Dispatcher.TryEnqueue(() =>
                {
                    ItemName = (m.SelectedItem?.Item?.Name) != string.Empty ? m.SelectedItem?.Item?.Name : localizer["NewItem"];
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

        App.Dispatcher.TryEnqueue(() =>
        {
            Currencies.Add(currency.BaseCurrency);
            Currencies.AddRange(currency.Rates.Keys);
        });
    }


    private async Task EnableEditing()
    {
        IsEditing = true;
    }

    private async Task<bool> Close()
    {
        if (IsEditing && !_isNew)
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

    private async Task Save()
    {
        WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, ToSave: true));
    }

    private async Task Archive()
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
