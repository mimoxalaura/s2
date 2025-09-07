using Unity.Services.Analytics;

namespace BackpackSurvivors.Game.Analytics.Events;

internal class GameStartedEvent : Event
{
	public string SteamId
	{
		set
		{
			SetParameter("skippedTutorialAfterTime", value);
		}
	}

	public GameStartedEvent()
		: base("backpackTutorialSkipped")
	{
	}
}
