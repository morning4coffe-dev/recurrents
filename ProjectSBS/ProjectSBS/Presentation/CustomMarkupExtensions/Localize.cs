using Microsoft.UI.Xaml.Markup;
using ProjectSBS.Presentation.Helpers;

namespace ProjectSBS.Presentation.CustomMarkupExtensions;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal class Localize : MarkupExtension
{
    public string? Name { get; set; }

    protected override object ProvideValue() =>
        string.IsNullOrEmpty(Name)
        ? string.Empty
        : Name.Localize();
}
