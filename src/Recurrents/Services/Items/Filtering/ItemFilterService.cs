namespace Recurrents.Services.Items.Filtering;

public class ItemFilterService : IItemFilterService
{
    public List<Tag> Categories { get; }
    public Tag SelectedCategory { get; set; }

    public ItemFilterService(IStringLocalizer localizer, ITagService tags)
    {
        Categories =
        [
            new Tag(-1, localizer["All"], null), .. tags.Tags
        ];
        SelectedCategory = Categories[0];
    }
}
