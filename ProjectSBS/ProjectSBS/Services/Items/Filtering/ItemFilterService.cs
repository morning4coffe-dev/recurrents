using ProjectSBS.Services.Items.Tags;

namespace ProjectSBS.Services.Items.Filtering;

public class ItemFilterService : IItemFilterService
{
    public List<Tag> Categories { get; }
    public Tag SelectedCategory { get; set; }

    public ItemFilterService(IStringLocalizer localizer, ITagService tags)
    {
        Categories = new()
        {
            new Tag(-1, localizer["All"], null)
        };
        Categories.AddRange(tags.Tags);
        SelectedCategory = Categories[0];
    }
}
