using Microsoft.AppCenter.Crashes;
using Service = Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;

namespace ProjectSBS.Services.Analytics;

public class AnalyticsService : IAnalyticsService
{
    public AnalyticsService()
    {
        AppCenter.Start("ea15cf3d-e346-483c-b0b3-618fa515fd50",
                  typeof(Service.Analytics), typeof(Crashes));

        Dictionary<string, string> appLaunchSettings = new()
            {
                { "123", $"Hello there" }
            };

        TrackEvent("App launched", appLaunchSettings);
    }

    public void TrackEvent(string eventName, IDictionary<string, string> properties = null)
    {
        Service.Analytics.TrackEvent(eventName, properties);
    }
}
