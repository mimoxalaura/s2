using Unity.Services.Analytics;

namespace BackpackSurvivors.Game.Analytics.Events;

internal class TownTutorialSkippedEvent : Event
{
	public float SkippedTutorialAfterTime
	{
		set
		{
			SetParameter("skippedTutorialAfterTime", value);
		}
	}

	public int ActiveTutorialEnumWhenSkipped
	{
		set
		{
			SetParameter("activeTutorialWhenSkipped", value);
		}
	}

	public TownTutorialSkippedEvent()
		: base("townTutorialSkipped")
	{
	}
}
