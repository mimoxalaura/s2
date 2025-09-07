using System;
using BackpackSurvivors.ScriptableObjects.Waves;

namespace BackpackSurvivors.Game.Waves.Events;

public class WaveCompletedEventArgs : EventArgs
{
	public WaveSO Wave { get; }

	public bool HasWavesRemaining { get; }

	public WaveCompletedEventArgs(WaveSO wave, bool hasWavesRemaining)
	{
		Wave = wave;
		HasWavesRemaining = hasWavesRemaining;
	}
}
