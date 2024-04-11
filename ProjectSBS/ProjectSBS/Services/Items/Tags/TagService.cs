using System.Drawing;

namespace ProjectSBS.Services.Items.Tags;

public class TagService(IStringLocalizer? localizer) : ITagService
{
    public List<Tag> Tags { get; } =
    [
        //Don't change the ID's, change only the order
        new(0, localizer?["News"] ?? "News", Color.FromArgb(200, 241, 90, 144)),
        new(1, localizer?["Entertainment"] ?? "Entertainment", Color.FromArgb(200, 205, 250, 219)),
        new(2, localizer?["Work"] ?? "Work", Color.FromArgb(200, 220, 52, 78)),
        new(3, localizer?["Food"] ?? "Food", Color.FromArgb(200, 109, 31, 73)),
        new(4, localizer?["Fitness"] ?? "Fitness", Color.FromArgb(200, 240, 186, 65)),
        new(5, localizer?["Education"] ?? "Education", Color.FromArgb(200, 120, 120, 242)),
        new(6, localizer?["Travel"] ?? "Travel", Color.FromArgb(200, 172, 125, 136)),
        new(7, localizer?["Health"] ?? "Health", Color.FromArgb(200, 139, 195, 74)),
        new(8, localizer?["Social"] ?? "Social", Color.FromArgb(200, 76, 175, 180)),
        new(9, localizer?["Finance"] ?? "Finance", Color.FromArgb(200, 250, 159, 166)),
        new(10, localizer?["Streaming"] ?? "Streaming", Color.FromArgb(200, 255, 152, 0)),
        new(11, localizer?["Gaming"] ?? "Gaming", Color.FromArgb(200, 0, 70, 255)),
        new(12, localizer?["Shopping"] ?? "Shopping", Color.FromArgb(200, 255, 204, 0)),
        new(13, localizer?["Productivity"] ?? "Productivity", Color.FromArgb(200, 0, 204, 102)),
        new(14, localizer?["Music"] ?? "Music", Color.FromArgb(200, 204, 0, 102)),
        new(15, localizer?["Photography"] ?? "Photography", Color.FromArgb(200, 51, 51, 255)),
        new(16, localizer?["Utilities"] ?? "Utilities", Color.FromArgb(200, 153, 0, 153)),
        new(17, localizer?["Fashion"] ?? "Fashion", Color.FromArgb(200, 255, 102, 102)),
        new(18, localizer?["Home"] ?? "Home", Color.FromArgb(200, 0, 153, 76)),
        new(19, localizer?["Books"] ?? "Books", Color.FromArgb(200, 255, 51, 153)),
        new(20, localizer?["Art"] ?? "Art", Color.FromArgb(200, 153, 153, 0)),
        new(21, localizer?["Software"] ?? "Software", Color.FromArgb(200, 0, 102, 204)),
        new(100, localizer?["Other"] ?? "Other", Color.FromArgb(200, 190, 40, 235)),
    ];
}
