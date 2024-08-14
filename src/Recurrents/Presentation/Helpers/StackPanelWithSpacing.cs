namespace Recurrents.Presentation.Helpers;

/// <summary>
/// Custom StackPanel Override: Resolves Spacing Issue
/// <see href="https://github.com/microsoft/microsoft-ui-xaml/issues/916"/>
/// The default StackPanel introduces spacing for collapsed items, leading to layout problems.
/// This custom implementation rectifies the issue by eliminating spacing for collapsed elements.
/// </summary>
public partial class StackPanelWithSpacing : StackPanel
{
    /// <summary>
    /// Gets or sets the space between visible elements.
    /// </summary>
    public static readonly DependencyProperty SpaceProperty =
        DependencyProperty.Register(nameof(Space), typeof(int), typeof(StackPanelWithSpacing), new PropertyMetadata(0));

    /// <summary>
    /// Initializes a new instance of the <see cref="StackPanelWithSpacing"/> class.
    /// </summary>
    public StackPanelWithSpacing()
    {
        Loaded += StackPanelWithSpacing_Loaded;
    }

    /// <summary>
    /// Gets or sets the space between visible elements.
    /// </summary>
    public int Space
    {
        get => (int)GetValue(SpaceProperty);
        set => SetValue(SpaceProperty, value);
    }

    private void StackPanelWithSpacing_Loaded(object sender, object e)
        => SetSpacingForChildren(Space);

    private void SetSpacingForChildren(int spacing)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] is FrameworkElement element
                && element.Visibility == Visibility.Visible)
            {
                var topSpacing = i == 0 ? 0 : spacing;

                element.Margin = new Thickness(element.Margin.Left, element.Margin.Top + topSpacing, element.Margin.Right, element.Margin.Bottom + (spacing / 2));
            }
        }
    }
}
