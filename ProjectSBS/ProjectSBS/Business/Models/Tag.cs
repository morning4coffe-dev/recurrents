using System.Drawing;

namespace ProjectSBS.Business.Models;

public class Tag 
{
    public int TagId { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }

    public Tag(
        int tagId, 
        string name, 
        Color color)
    {
        TagId = tagId;
        Name = name;
        Color = color;
    }
}
