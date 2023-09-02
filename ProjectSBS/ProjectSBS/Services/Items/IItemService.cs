using ProjectSBS.Presentation.Components;

namespace ProjectSBS.Services.Items;

public interface IItemService
{
    ItemViewModel ScheduleBilling(ItemViewModel item);
}