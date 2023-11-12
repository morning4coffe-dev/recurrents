namespace ProjectSBS.Services.Items.Filtering;

public class ItemFilterService : IItemFilterService
{
    public List<FilterCategory> Categories { get; }
    public FilterCategory SelectedCategory { get; set; }

    public ItemFilterService(IStringLocalizer localizer)
    {
        //TODO: Add proper Selectors for FilterCategories
        Categories = new()
        {
            new(localizer["All"], "\uE80F"),
            new(localizer["Home"], "\uE752"),
            new(localizer["Overdue"], "\uEC92", i => i.Item?.Name is "Sample Item 1"),
            new(localizer["Expensive"], "\uE717", i => i.Item?.Billing.BasePrice > 50),
        };

        SelectedCategory = Categories[0];
    }
}
