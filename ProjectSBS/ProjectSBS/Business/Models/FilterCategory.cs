namespace ProjectSBS.Business.Models;

public enum CategoryVisibility
{
    Both,
    Desktop,
    Mobile
}

public class FilterCategory
{
    public FilterCategory(string text, string? glyph, Func<ItemViewModel, bool>? selector = null)
    {
        Text = text;
        Glyph = glyph;
        Selector = selector;
    }

    public string Text { get; }
    public string? Glyph { get; }
    public Func<ItemViewModel, bool>? Selector { get; }
}
