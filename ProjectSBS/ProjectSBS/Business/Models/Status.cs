namespace ProjectSBS.Business.Models;

public enum State 
{
    Active,
    Archived
}

public record Status
{
    public State State { get; set; }
    public DateOnly Date { get; set; }
}
