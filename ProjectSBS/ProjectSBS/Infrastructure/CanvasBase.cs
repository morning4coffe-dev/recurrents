using Windows.UI.ViewManagement;

namespace ProjectSBS.Infrastructure;

public class CanvasBase : ViewModelBase
{
    private readonly UISettings _themeListener = new();

    public virtual void UISettings_ColorValuesChanged(UISettings sender, object args)
    { 
    
    }

    public override void Load()
    {
        _themeListener.ColorValuesChanged += UISettings_ColorValuesChanged;
    }

    public override void Unload()
    {
        _themeListener.ColorValuesChanged -= UISettings_ColorValuesChanged;
    }
}
