using System;

namespace BackpackSurvivors.System.Settings.Events;

public class FlashOnDamageTakenSettingsChangedEventArgs : EventArgs
{
	public Enums.FlashOnDamageTaken FlashOnDamageTaken { get; }

	public FlashOnDamageTakenSettingsChangedEventArgs(Enums.FlashOnDamageTaken flashOnDamageTaken)
	{
		FlashOnDamageTaken = flashOnDamageTaken;
	}
}
