namespace Recurrents.Presentation;

public partial class ItemEditViewModel : ObservableObject
{
    #region Services
    private readonly IStringLocalizer _localizer;
    #endregion

    [ObservableProperty]
    private Item? _editItem;

    public ObservableCollection<string> Currencies { get; } = [];
    public ObservableCollection<string> PaymentMethods { get; }

    public ItemEditViewModel(ItemViewModel itemViewModel, IStringLocalizer localizer)
    {
        _localizer = localizer;

        EditItem = itemViewModel.Item;

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

    [RelayCommand]
    private void Save()
    {
        //if (EditItem is not { } item || SelectedItem is not { })
        //{
        //    return;
        //}

        //SelectedItem.Item = item;
        //WeakReferenceMessenger.Default.Send(new ItemUpdated(SelectedItem, ToSave: true));
    }
}
