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
        new(3, localizer?["Food"] ?? "Food", Color.FromArgb(200, 255, 62, 165)),
        new(4, localizer?["Fitness"] ?? "Fitness", Color.FromArgb(200, 240, 186, 65)),
        new(5, localizer?["Education"] ?? "Education", Color.FromArgb(200, 120, 120, 242)),
        new(6, localizer?["Travel"] ?? "Travel", Color.FromArgb(200, 172, 125, 136)),
        new(7, localizer?["Health"] ?? "Health", Color.FromArgb(200, 139, 195, 74)),
        new(8, localizer?["Social"] ?? "Social", Color.FromArgb(200, 76, 175, 180)),
        new(9, localizer?["Finance"] ?? "Finance", Color.FromArgb(200, 250, 159, 166)),
        new(100, localizer?["Other"] ?? "Other", Color.FromArgb(200, 190, 40, 235)),
    ];
}
