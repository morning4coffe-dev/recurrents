namespace Recurrents.Presentation;

public partial record SecondViewModel(Entity Entity)
{
    public Entity[] Items { get; } =
    [
        new Entity("First"),
            new Entity("Second"),
            new Entity("Third"),
            new Entity("Fourth"),
        ];
}
