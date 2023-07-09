#if __ANDROID__

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using AndroidX.Core.App;
using Uno.Extensions;

namespace ProjectSBS.Services.Notifications;

//TODO Web specific notifications

public class NotificationService : NotificationServiceBase
{

    public NotificationService()
    {
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {
        InvokeInAppNotificationRequested(new InAppNotificationRequestedEventArgs { NotificationText = notification, NotificationTime = autoHide ? 1500 : 0 });
    }

    public override void ScheduleNotification(string title, string text, DateTime day, TimeSpan notificationTime)
    {
        throw new NotImplementedException();
    }

    public override void RemoveScheduledNotifications(bool onlyRemoveToday = false)
    {
        throw new NotImplementedException();
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

        var channelId = "projectSBS-channel";
        var channelName = "Reminders";
        var importance = NotificationImportance.Default;

        var notificationChannel = new NotificationChannel(channelId, channelName, importance);
        notificationManager.CreateNotificationChannel(notificationChannel);

        var notificationBuilder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
            .SetSmallIcon(Resource.Drawable.notification_icon_background)
            .SetContentTitle(title)
            .SetContentText(description)
            .SetAutoCancel(true);

        var notification = notificationBuilder.Build();

        notificationManager.Notify(0, notification);
    }

}
#endif
