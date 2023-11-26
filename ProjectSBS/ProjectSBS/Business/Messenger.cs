namespace ProjectSBS.Business;

public record ItemSelectionChanged(ItemViewModel? SelectedItem, bool IsEdit = false, bool IsNew = false);
public record ItemUpdated(ItemViewModel Item, bool Canceled = false, bool ToSave = false);
public record ItemArchived(ItemViewModel Item);
public record CategorySelectionChanged();
