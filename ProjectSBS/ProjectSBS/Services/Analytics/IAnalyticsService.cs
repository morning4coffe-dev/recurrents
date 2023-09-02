namespace ProjectSBS.Services.Analytics;

public interface IAnalyticsService
{
    void TrackEvent(string eventName, IDictionary<string, string> properties = null);
}