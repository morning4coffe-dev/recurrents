namespace ProjectSBS.Business.Models;

public class NavigationCategory
{
    public NavigationCategory(uint id, string text, string? glyph, Type page, CategoryVisibility visibility = CategoryVisibility.Both)
    {
        Id = id;
        Text = text;
        Glyph = glyph;
        Page = page;
        Visibility = visibility;
    }

    public uint Id { get; }
    public string Text { get; }
    public string? Glyph { get; }
    public Type Page { get; }
    public CategoryVisibility Visibility { get; }
}
