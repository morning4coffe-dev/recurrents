using ProjectSBS.Services.Items;

namespace ProjectSBS;

public partial class ItemViewModel : ObservableObject
{

    public ItemViewModel(Item item)
    {
        _item = item;

        (Application.Current as App).Host?.Services.GetService<IItemService>();
    }

    [ObservableProperty]
    private Item _item;
}
