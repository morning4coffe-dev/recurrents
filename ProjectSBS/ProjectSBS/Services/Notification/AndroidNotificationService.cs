#if __ANDROID__

using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Lifecycle;
using System;

namespace ProjectSBS.Services.Notifications;

public class AndroidNotificationService : NotificationServiceBase
{
    private Context context = Android.App.Application.Context;

    public AndroidNotificationService()
    {
    }

    public override bool IsEnabledOnDevice()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
        {
            //TODO Returns true on all
            //return true;
        }

        return NotificationManagerCompat.From(context).AreNotificationsEnabled();
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {
        InvokeInAppNotificationRequested(new InAppNotificationRequestedEventArgs
        {
            NotificationText = notification,
            NotificationTime = autoHide ? 1500 : 0
        });
    }

    public override async void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time)
    {
        if (!IsEnabledOnDevice())
        {
            //TODO make this not only for not scheduled notifications  
            var current = await ApplicationActivity.GetCurrent(new CancellationToken());
            if (ActivityCompat.CheckSelfPermission(current, Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(current, new string[] { Manifest.Permission.PostNotifications }, 1);
            }
        }

        id = Guid.NewGuid().ToString();

        DateTime notificationDateTime = new(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second);
        long totalMilliSeconds = (long)(notificationDateTime.ToUniversalTime() - DateTime.Now).TotalMilliseconds;

        text += $" Scheduled for {notificationDateTime}";

        var (manager, intent) = CreateAlarm(id, title, text, notificationDateTime);

        GetAlarm();

        manager.SetExact(AlarmType.ElapsedRealtime, totalMilliSeconds, intent);
    }

    private (AlarmManager, PendingIntent) CreateAlarm(string id, string title, string text, DateTime notificationDateTime)
    {
        if (notificationDateTime > DateTime.Now)
        {
            Intent notificationIntent = new(context, typeof(NotificationReceiver));
            notificationIntent.PutExtra("id", id);
            notificationIntent.PutExtra("title", title);
            notificationIntent.PutExtra("text", text);

            var random = new Random();
            int requestCode = random.Next(0, 5000); // TODO: ID here You can convert the id to an integer for the requestCode

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, requestCode, notificationIntent, PendingIntentFlags.Immutable);

            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            return (alarmManager, pendingIntent);
        }

        //TODO throw new Exception("Desired time was set in the past.");
        return (null, null);
    }

    // TODO: Test this function properly
    public override void RemoveScheduledNotifications(string id)
    {
        Intent notificationIntent = new(context, typeof(NotificationReceiver));
        notificationIntent.PutExtra("id", id);

        var random = new Random();
        int requestCode = random.Next(0, 5000); // TODO: ID here You can convert the id to an integer for the requestCode

        PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, requestCode, notificationIntent, PendingIntentFlags.Immutable);

        AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

        alarmManager.Cancel(pendingIntent);
    }

    private void GetAlarm() 
    {
        AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

        //var c = alarmManager.CanScheduleExactAlarms();
        //var d = alarmManager.NextAlarmClock;
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

        var channelId = "ProjectSBS-channel";
        var channelName = "Other";
        var importance = NotificationImportance.High;

        var notificationChannel = new NotificationChannel(channelId, channelName, importance);
        notificationManager.CreateNotificationChannel(notificationChannel);

        var notificationBuilder = new NotificationCompat.Builder(context, channelId)
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

        var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

        PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);

        var channelId = id;
        var channelName = id; // TODO: Change from id to Item name
        var importance = NotificationImportance.High;

        var notificationChannel = new NotificationChannel(channelId, channelName, importance);
        notificationManager.CreateNotificationChannel(notificationChannel);

        var notificationBuilder = new NotificationCompat.Builder(context, channelId)
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
