using Microsoft.AppCenter.Crashes;
using Service = Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;

namespace ProjectSBS.Services.Analytics;

public class AnalyticsService
{
    public const string Launched = "01_Launched";
    public const string LogIn = "02_LogIn";
    public const string ItemEvent = "03_ItemEvent";

    public static void Initialize()
    {
#if !HAS_UNO && !DEBUG
        AppCenter.Start(AppConfig.TelemetryId,
                  typeof(Service.Analytics), typeof(Crashes));
#endif
    }

    public static void TrackEvent(string eventName, IDictionary<string, string>? properties = null)
    {
#if !HAS_UNO && !DEBUG
        Service.Analytics.TrackEvent(eventName, properties);
#endif
    }

    public static void TrackEvent(string eventName, string valueName, string value)
    {
#if !HAS_UNO && !DEBUG
        TrackEvent(eventName, new Dictionary<string, string>() { { valueName, value } });
#endif
    }
}

