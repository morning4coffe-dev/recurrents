namespace ProjectSBS.Infrastructure;

public abstract class ViewModelBase : ObservableObject
{
    public abstract void Load();
    public abstract void Unload();
}
