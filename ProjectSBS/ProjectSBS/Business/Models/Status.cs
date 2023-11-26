namespace ProjectSBS.Business.Models;

public enum State 
{
    Active,
    Archived
}

public record Status(State State, DateOnly Date)
{
    public State State { get; set; } = State;
    public DateOnly Date { get; set; } = Date;
}
