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
           new(0, localizer["Video"], Color.FromArgb(200, 159, 187, 115)),
           new(1, localizer["Music"], Color.FromArgb(200, 241, 235, 144)),
           new(2, localizer["Gaming"], Color.FromArgb(200, 190, 49, 68)),
           new(3, localizer["TV"], Color.FromArgb(200, 240, 86, 65)),
        };
    }
}
