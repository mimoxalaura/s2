using Unity.Services.Analytics;

namespace BackpackSurvivors.Game.Analytics.Events;

internal class BackpackTutorialSkippedEvent : Event
{
	public float SkippedTutorialAfterTime
	{
		set
		{
			SetParameter("skippedTutorialAfterTime", value);
		}
	}

	public BackpackTutorialSkippedEvent()
		: base("backpackTutorialSkipped")
	{
	}
}
