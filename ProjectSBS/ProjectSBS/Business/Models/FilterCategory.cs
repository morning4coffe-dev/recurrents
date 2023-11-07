namespace ProjectSBS.Business.Models;

public class FilterCategory
{
    public FilterCategory(string text, string? glyph, Func<ItemViewModel, bool>? selector = null, CategoryVisibility visibility = CategoryVisibility.Both)
    {
        Text = text;
        Glyph = glyph;
        Selector = selector;
        Visibility = visibility;
    }

    public string Text { get; }
    public string? Glyph { get; }
    public Func<ItemViewModel, bool>? Selector { get; }
    public CategoryVisibility Visibility { get; }
}
