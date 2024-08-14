using System.Drawing;

namespace Recurrents.Business.Models;

public class Tag(
    int id,
    string name,
    Color? color)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Color? Color { get; set; } = color;
}
