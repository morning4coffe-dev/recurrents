#if !HAS_UNO
using CommunityToolkit.WinUI.Notifications;
using System;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;

namespace ProjectSBS.Services.Notifications;

public class WindowsNotificationService : NotificationServiceBase
{
    public override bool IsEnabledOnDevice()
    {
        ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();

        return notifier.Setting is NotificationSetting.Enabled;
    }

    public override void ShowInAppNotification(string notification, bool autoHide)
    {
        InvokeInAppNotificationRequested(new InAppNotificationRequestedEventArgs { NotificationText = notification, NotificationTime = autoHide ? 1500 : 0 });
    }

    public override void ShowBasicToastNotification(string title, string description)
    {
        if (!IsEnabledOnDevice())
        {
            //TODO show that notification could not been sent
            return;
        }

        var assetUri = AppDomain.CurrentDomain.BaseDirectory + "Assets";

        new ToastContentBuilder()
            //.AddInlineImage(new Uri("file:///" + assetUri + "/Icon_Nice.png"))
            .AddText(title)
            .AddText(description)
            .Show();
    }

    public override void ScheduleNotification(string id, string title, string text, DateOnly day, TimeOnly time)
    {
        if (!IsEnabledOnDevice())
        {
            //TODO show that notification could not been sent
            return;
        }

        var date = new DateTime(day.Year, day.Month, day.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Unspecified);

        if (date < DateTime.Now)
        {
            return;
        }


        CreateNotification(id, title, text).Schedule(date, toast =>
            {
                //toast.Id = new Guid().ToString().Take(4).ToString();
            });
    }

    private ToastContentBuilder CreateNotification(string id, string title, string text)
    {
        return new ToastContentBuilder()
            //.AddArgument("action", "viewItemsDueToday")
            .AddInlineImage(new Uri("ms-appx:///Assets/app-icon.png"))
            .AddText(title)
            .AddText(text)
            .AddArgument("action", "viewItem")
            .AddArgument("itemId", id)
            .AddToastInput(new ToastSelectionBox("snoozeTime")
            {
                DefaultSelectionBoxItemId = "15",
                Items =
                {
                    new ToastSelectionBoxItem("5", "5 minutes"),
                    new ToastSelectionBoxItem("15", "15 minutes"),
                    new ToastSelectionBoxItem("60", "1 hour"),
                    new ToastSelectionBoxItem("240", "4 hours"),
                    new ToastSelectionBoxItem("1440", "1 day")
                }
            })
            .AddButton(new ToastButtonSnooze() { SelectionBoxId = "snoozeTime" })
            .AddButton(
            new ToastButton()
                .SetContent("Done")
                .AddArgument("action", "done")
                .SetBackgroundActivation()
                )
            .SetToastScenario(ToastScenario.Reminder);
    }

    public override void RemoveScheduledNotifications(string id = "")
    {
        if (string.IsNullOrEmpty(id))
        {
            ToastNotificationManagerCompat.History.Clear();
            return;
        }

        ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();
        IReadOnlyList<ScheduledToastNotification> scheduledToasts = notifier.GetScheduledToastNotifications();

        foreach (var toRemove in scheduledToasts)
        {
            if (toRemove.Id == id)
            {
                notifier.RemoveFromSchedule(toRemove);
            }
        }

    }
}
#endif
