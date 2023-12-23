using Microsoft.AppCenter.Crashes;
using Service = Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;

namespace ProjectSBS.Services.Analytics;

public class AnalyticsService : IAnalyticsService
{
    public AnalyticsService()
    {
#if !HAS_UNO
        AppCenter.Start("ea15cf3d-e346-483c-b0b3-618fa515fd50",
                  typeof(Service.Analytics), typeof(Crashes));
#endif
    }

    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null)
    {
#if !HAS_UNO
        Service.Analytics.TrackEvent(eventName, properties);
#endif
    }
}
