namespace ProjectSBS.Business;

public record ItemSelectionChanged(ItemViewModel? SelectedItem, bool IsEdit = false);
public record ItemUpdated(ItemViewModel? Item);
