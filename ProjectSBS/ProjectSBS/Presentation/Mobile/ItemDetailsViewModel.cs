using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;

namespace ProjectSBS.Presentation.Mobile;

public partial class ItemDetailsViewModel : ObservableObject
{
    private INavigator _navigator;

    private static bool _messengerLock;

    [ObservableProperty]
    private ItemViewModel _selectedItem;

    [ObservableProperty]
    private bool _isEditing = false;

    public ICommand EnableEditingCommand { get; }

    public ItemDetailsViewModel(INavigator navigator)
    {
        _navigator = navigator;

        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);

        if (!_messengerLock)
        {
            WeakReferenceMessenger.Default.Register<ItemSelectionChanged>(this, (r, m) =>
            {
                SelectedItem = m.Item;

                if (SelectedItem.Item is null)
                {
                    IsEditing = true;
                }

                return;
            });
            _messengerLock = true;
        }
    }

    private async Task EnableEditing()
    {
        IsEditing = true;
    }
}