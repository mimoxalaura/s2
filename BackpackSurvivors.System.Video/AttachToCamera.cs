using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.System.Video;

[RequireComponent(typeof(Camera))]
internal class AttachToCamera : MonoBehaviour
{
	[SerializeField]
	private string _targetCameraNameToFind;

	private void Start()
	{
		Camera component = GetComponent<Camera>();
		GameObject gameObject = GameObject.Find(_targetCameraNameToFind);
		if (!(gameObject == null))
		{
			Camera component2 = gameObject.GetComponent<Camera>();
			if (!(component2 == null))
			{
				component.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(component2);
			}
		}
	}
}
