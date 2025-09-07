namespace BackpackSurvivors.System.Helper;

internal class BuildInfoHelper
{
	internal static string GetUnityAnalyticsEnvironmentName(Enums.BuildInfo.UnityAnalyticsEnvironment environment)
	{
		return environment switch
		{
			Enums.BuildInfo.UnityAnalyticsEnvironment.Development => "development", 
			Enums.BuildInfo.UnityAnalyticsEnvironment.DemoProduction => "demo-production", 
			Enums.BuildInfo.UnityAnalyticsEnvironment.AnalyticsTest => "analytics-test", 
			Enums.BuildInfo.UnityAnalyticsEnvironment.Production => "production", 
			_ => "development", 
		};
	}
}
