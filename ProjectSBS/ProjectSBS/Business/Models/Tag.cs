using System.Drawing;

namespace ProjectSBS.Business.Models;

//public record Tag(
//    string Name,
//    Color Color)
//{

//}

public class Tag 
{
    public string Name { get; set; }
    public Color Color { get; set; }

    public Tag(string name, Color color)
    {
        Name = name;
        Color = color;
    }
}
