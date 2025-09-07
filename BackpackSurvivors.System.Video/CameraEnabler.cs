using UnityEngine;

namespace BackpackSurvivors.System.Video;

internal class CameraEnabler : MonoBehaviour
{
	[SerializeField]
	private Camera[] _cameras;

	internal void SetCamerasEnabled(bool enabled)
	{
		Camera[] cameras = _cameras;
		for (int i = 0; i < cameras.Length; i++)
		{
			cameras[i].gameObject.SetActive(enabled);
		}
	}
}
