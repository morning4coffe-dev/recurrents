﻿namespace ProjectSBS.Services.Items;

public interface IItemService
{
    Task InitializeAsync();
    IEnumerable<ItemViewModel> GetItems(Func<ItemViewModel, bool>? selector = null);
    void NewItem(Item item, List<ItemLog>? logs = null);
    void DeleteItem(ItemViewModel item);
    ItemViewModel ScheduleBilling(ItemViewModel item, List<ItemLog> logs);
}