using System;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace BackpackSurvivors.Game.Analytics;

internal class AnalyticsController : SingletonController<AnalyticsController>
{
	public override void AfterBaseAwake()
	{
		base.AfterBaseAwake();
		UnityServices.InitializeAsync();
	}

	private void Start()
	{
		SetEvironment();
		StartLogging();
	}

	private async void SetEvironment()
	{
		InitializationOptions initializationOptions = new InitializationOptions();
		string unityAnalyticsEnvironmentName = BuildInfoHelper.GetUnityAnalyticsEnvironmentName(SingletonController<GameDatabase>.Instance.GameDatabaseSO.UnityAnalyticsEnvironment);
		initializationOptions.SetEnvironmentName(unityAnalyticsEnvironmentName);
		await UnityServices.InitializeAsync(initializationOptions);
	}

	private void StartLogging()
	{
		AnalyticsService.Instance.StartDataCollection();
	}

	private void StopLogging()
	{
		AnalyticsService.Instance.StopDataCollection();
	}

	internal void RecordEvent<T>(T eventToRecord) where T : Event
	{
		try
		{
			AnalyticsService.Instance.RecordEvent(eventToRecord);
		}
		catch (Exception)
		{
		}
	}
}
