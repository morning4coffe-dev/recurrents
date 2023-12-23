namespace ProjectSBS.Services.Analytics;

public interface IAnalyticsService
{
    void TrackEvent(string eventName, IDictionary<string, string> properties = null);
}

public class AnalyticsConst
{ 
    public const string Launched = "01_Launched";
    public const string LoggedIn = "02_LoggedIn";
}
