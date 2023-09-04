using CommunityToolkit.Mvvm.Messaging;
using ProjectSBS.Business;
using Windows.UI.Core;

namespace ProjectSBS.Presentation.Mobile;

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

    public ICommand EnableEditingCommand { get; }
    public ICommand CloseCommand { get; }

    public ItemDetailsViewModel(INavigator navigator, IDispatcher dispatch)
    {
        _navigator = navigator;
        _dispatch = dispatch;

        EnableEditingCommand = new AsyncRelayCommand(EnableEditing);
        CloseCommand = new AsyncRelayCommand(Close);

#if HAS_UNO
        SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
#endif

        SelectedItem = MainViewModel.SentItem;

        if (SelectedItem?.Item is null)
        {
            IsEditing = true;
        }

        ItemName = SelectedItem?.Item?.Name ?? "New Item";
    }

    private void System_BackRequested(object? sender, BackRequestedEventArgs e)
    {
        e.Handled = true;
        _ = Close();
        SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
    }

    private async Task EnableEditing()
    {
        IsEditing = true;
    }

    private async Task Close()
    {
        //TODO Ask to save?

        WeakReferenceMessenger.Default.Send(new ItemSelectionChanged(SelectedItem));

        await _navigator.NavigateBackAsync(this);
    }
}