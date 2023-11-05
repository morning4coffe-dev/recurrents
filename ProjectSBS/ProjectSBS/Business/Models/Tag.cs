using System.Drawing;

namespace ProjectSBS.Business.Models;

public class Tag 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Color Color { get; set; }

    public Tag(
        int id, 
        string name, 
        Color color)
    {
        Id = id;
        Name = name;
        Color = color;
    }
}
