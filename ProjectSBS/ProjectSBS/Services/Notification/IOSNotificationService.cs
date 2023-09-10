#if __IOS__
namespace ProjectSBS.Services.Notifications;

public class IOSNotificationService : NotificationServiceBase
{
    public IOSNotificationService()
    {
    }

    public override bool IsEnabledOnDevice()
    {
        return false;
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {

    }

    public override async void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time)
    {

    }

    // TODO: Test this function properly
    public override void RemoveScheduledNotifications(string id)
    {

    }

    public override void ShowBasicToastNotification(string title, string description)
    {

    }
}
#endif
