using System;

namespace BackpackSurvivors.System.Settings.Events;

public class CameraShakeChangedEventArgs : EventArgs
{
	public Enums.CameraShake CameraShake { get; }

	public CameraShakeChangedEventArgs(Enums.CameraShake cameraShake)
	{
		CameraShake = cameraShake;
	}
}
