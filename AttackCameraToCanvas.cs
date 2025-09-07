using System.Linq;
using UnityEngine;

public class AttackCameraToCanvas : MonoBehaviour
{
	[SerializeField]
	private string _cameraName;

	internal void Refresh()
	{
		Camera camera = Object.FindObjectsOfType<Camera>().FirstOrDefault((Camera x) => x.name == _cameraName);
		if (camera != null)
		{
			GetComponent<Canvas>().worldCamera = camera;
		}
	}

	private void Start()
	{
		Refresh();
	}
}
