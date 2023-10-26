using Microsoft.UI.Xaml.Media.Imaging;

namespace ProjectSBS.Business.Models;

public class User
{
    public User(string name, string email, BitmapImage? photo)
    {
        Name = name;
        Email = email;
        Photo = photo;
    }

    public string Name { get; }

    public string Email { get; }

    public BitmapImage? Photo { get; set; }
}
