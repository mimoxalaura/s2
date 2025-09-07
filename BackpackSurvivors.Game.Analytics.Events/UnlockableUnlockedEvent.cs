using Unity.Services.Analytics;

namespace BackpackSurvivors.Game.Analytics.Events;

internal class UnlockableUnlockedEvent : Event
{
	public string UnlockableName
	{
		set
		{
			SetParameter("UnlockableName", value);
		}
	}

	public int UnlockablePointsInvested
	{
		set
		{
			SetParameter("UnlockablePointsInvested", value);
		}
	}

	public UnlockableUnlockedEvent()
		: base("unlockableUnlocked")
	{
	}
}
