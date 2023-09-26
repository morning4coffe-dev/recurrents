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
           new(0, localizer["Red Welcome"], Color.FromName("Red")),
           new(1, localizer["Blue Welcome"], Color.FromName("Blue")),
           new(2, localizer["Yellow Welcome"], Color.FromName("Yellow"))
        };
    }
}
