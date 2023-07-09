#if __WASM__

using Uno;

namespace ProjectSBS.Services.Notifications;

public class WindowsNotificationService : NotificationServiceBase
{

    public WindowsNotificationService()
    {
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {
        InvokeInAppNotificationRequested(new InAppNotificationRequestedEventArgs { NotificationText = notification, NotificationTime = autoHide ? 1500 : 0 });
    }

    public override void ScheduleNotification(string title, string text, DateTime day, TimeSpan notificationTime)
    {

    }

    public override void RemoveScheduledNotifications(bool onlyRemoveToday = false)
    {

    }

    public override void ShowBasicToastNotification(string title, string description)
    {

    }

}
#endif
