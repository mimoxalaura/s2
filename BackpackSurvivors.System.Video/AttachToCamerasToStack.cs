using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.System.Video;

internal class AttachToCamerasToStack : MonoBehaviour
{
	[SerializeField]
	private SerializableDictionaryBase<int, string> _targetCameraNamesToFindOrdered;

	private void Start()
	{
		Camera component = GetComponent<Camera>();
		Camera[] array = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		foreach (KeyValuePair<int, string> item in _targetCameraNamesToFindOrdered.OrderBy((KeyValuePair<int, string> x) => x.Key))
		{
			Camera[] array2 = array;
			foreach (Camera camera in array2)
			{
				if (camera.name.ToLower() == item.Value.ToLower())
				{
					component.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(camera);
					camera.rect = component.rect;
				}
			}
		}
	}
}
