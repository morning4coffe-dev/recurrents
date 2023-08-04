#if __ANDROID__

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using AndroidX.Core.App;
using Uno.Extensions;

namespace ProjectSBS.Services.Notifications;

//TODO Rename to AndroidNotificationService

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
        DateTime notificationDateTime = new(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second);

        var (manager, intent) = CreateAlarm(id, title, text, notificationDateTime);

        manager.Set(AlarmType.RtcWakeup, notificationDateTime.Millisecond, intent);

    }

    private (AlarmManager, PendingIntent) CreateAlarm(string id, string title, string text, DateTime notificationDateTime)
    {
        if (notificationDateTime > DateTime.Now)
        {
            Intent notificationIntent = new(Android.App.Application.Context, typeof(NotificationReceiver));
            notificationIntent.PutExtra("id", id);
            notificationIntent.PutExtra("title", title);
            notificationIntent.PutExtra("text", text);

            var random = new Random();
            int requestCode = random.Next(0, 5000); //TODO: ID here You can convert the id to an integer for the requestCode

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Android.App.Application.Context, requestCode, notificationIntent, PendingIntentFlags.Immutable);

            AlarmManager alarmManager = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);

            return (alarmManager, pendingIntent);
        }

        throw new Exception("Desired time was set in the past.");
    }

    public override void RemoveScheduledNotifications(string id)
    {
        //throw new NotImplementedException();
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

        var channelId = "ProjectSBS-channel";
        var channelName = "Other";
        var importance = NotificationImportance.High;

        var notificationChannel = new NotificationChannel(channelId, channelName, importance);
        notificationManager.CreateNotificationChannel(notificationChannel);

        var notificationBuilder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
            .SetSmallIcon(Resource.Drawable.abc_vector_test)
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

        var channelId = id;
        var channelName = id; //TODO Change from id to Item name
        var importance = NotificationImportance.High;

        var notificationChannel = new NotificationChannel(channelId, channelName, importance);
        notificationManager.CreateNotificationChannel(notificationChannel);

        var notificationBuilder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
            .SetSmallIcon(Resource.Drawable.abc_vector_test)
            .SetContentTitle(title)
            .SetContentText(text)
            .SetPriority(NotificationCompat.PriorityHigh)
            .SetContentIntent(pendingIntent)
            .SetAutoCancel(true);

        var notification = notificationBuilder.Build();

        notificationManager.Notify(0, notification);
    }
}
#endif
