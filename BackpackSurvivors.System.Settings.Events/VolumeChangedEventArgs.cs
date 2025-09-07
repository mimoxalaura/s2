using System;

namespace BackpackSurvivors.System.Settings.Events;

public class VolumeChangedEventArgs : EventArgs
{
	public Enums.AudioType AudioType { get; }

	public float NewValue { get; }

	public VolumeChangedEventArgs(Enums.AudioType audioType, float newValue)
	{
		AudioType = audioType;
		NewValue = newValue;
	}
}
