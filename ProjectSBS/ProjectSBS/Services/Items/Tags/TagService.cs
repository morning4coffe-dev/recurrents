using System.Drawing;

namespace ProjectSBS.Services.Items.Tags;

public class TagService : ITagService
{
    public List<Tag> Tags { get; }

    public TagService(IStringLocalizer localizer)
    {
        //TODO: Add proper Selectors for FilterCategories
        Tags = new()
        {
           new Tag(localizer["Welcome"], Color.FromName("Red")),
           new Tag(localizer["Welcome"], Color.FromName("Red")),
           new Tag(localizer["Welcome"], Color.FromName("Red"))
        };
    }
}
