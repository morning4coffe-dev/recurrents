namespace ProjectSBS.Business.Models;

public class User
{
    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; }

    public string Email { get; }
}
