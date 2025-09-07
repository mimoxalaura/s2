using System;

namespace BackpackSurvivors.System.Settings.Events;

public class VideoSettingsChangedEventArgs : EventArgs
{
	public Enums.VideoSettingsType VideoSettingsType { get; }

	public int NewValue { get; }

	public VideoSettingsChangedEventArgs(Enums.VideoSettingsType videoSettingsType, int newValue)
	{
		VideoSettingsType = videoSettingsType;
		NewValue = newValue;
	}
}
