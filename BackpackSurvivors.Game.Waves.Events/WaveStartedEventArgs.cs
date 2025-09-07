using System;
using BackpackSurvivors.ScriptableObjects.Waves;

namespace BackpackSurvivors.Game.Waves.Events;

public class WaveStartedEventArgs : EventArgs
{
	public WaveSO Wave { get; }

	public WaveStartedEventArgs(WaveSO wave)
	{
		Wave = wave;
	}
}
