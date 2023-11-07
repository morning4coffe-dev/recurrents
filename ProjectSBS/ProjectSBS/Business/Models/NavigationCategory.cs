namespace ProjectSBS.Business.Models;

public class NavigationCategory
{
    public NavigationCategory(string text, string? glyph, Type page, CategoryVisibility visibility = CategoryVisibility.Both)
    {
        Text = text;
        Glyph = glyph;
        Page = page;
        Visibility = visibility;
    }

    public string Text { get; }
    public string? Glyph { get; }
    public Type Page { get; }
    public CategoryVisibility Visibility { get; }
}
