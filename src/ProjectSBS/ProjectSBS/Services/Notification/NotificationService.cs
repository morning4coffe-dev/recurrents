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

    public override void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time)
    {
        // Combine the date and time to create a single DateTime object
        DateTime notificationDateTime = new DateTime(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second);

        // Check if the notification time is in the future
        if (notificationDateTime > DateTime.Now)
        {
            Intent notificationIntent = new Intent(Android.App.Application.Context, typeof(NotificationReceiver));
            notificationIntent.PutExtra("id", id);
            notificationIntent.PutExtra("title", title);
            notificationIntent.PutExtra("text", text);

            int requestCode = 22; //TODO: ID here You can convert the id to an integer for the requestCode

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, requestCode, notificationIntent, PendingIntentFlags.Immutable);

            AlarmManager alarmManager = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
            //alarmManager.Set(AlarmType.RtcWakeup, notificationDateTime.Millisecond, pendingIntent);

            alarmManager.SetRepeating(AlarmType.RtcWakeup, notificationDateTime.Millisecond, TimeOnly.FromDateTime(DateTime.Now).Add(TimeSpan.FromSeconds(60)).Millisecond, pendingIntent);
        }
    }

    public override void RemoveScheduledNotifications(string id)
    {
        //throw new NotImplementedException();
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

        var channelId = "ProjectSBS-channel";
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

[BroadcastReceiver(Enabled = true)]
public class NotificationReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        string id = intent.GetStringExtra("id");
        string title = intent.GetStringExtra("title");
        string text = intent.GetStringExtra("text");


        var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

        PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, 0, intent, PendingIntentFlags.Immutable);

        var channelId = "ProjectSBS-channel";
        var channelName = "Reminders";
        var importance = NotificationImportance.Default;

        var notificationChannel = new NotificationChannel(channelId, channelName, importance);
        notificationManager.CreateNotificationChannel(notificationChannel);

        var notificationBuilder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
            .SetSmallIcon(Resource.Drawable.notification_icon_background)
            .SetContentTitle(title)
            .SetContentText(text)
            .SetPriority(NotificationCompat.PriorityHigh)
            // Set the intent that will fire when the user taps the notification
            .SetContentIntent(pendingIntent)
            .SetAutoCancel(true);

        var notification = notificationBuilder.Build();

        notificationManager.Notify(0, notification);
    }
}
#endif
