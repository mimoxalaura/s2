using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

public class ParallaxBackground : MonoBehaviour
{
	[Header("Base")]
	[SerializeField]
	private float parallaxFactor = 0.5f;

	[SerializeField]
	private bool isVertical;

	private Transform cameraTransform;

	private Vector3 lastCameraPosition;

	private void Start()
	{
		cameraTransform = Camera.main.transform;
		lastCameraPosition = cameraTransform.position;
	}

	private void Update()
	{
		Vector3 vector = cameraTransform.position - lastCameraPosition;
		if (isVertical)
		{
			base.transform.position += new Vector3(0f, vector.y * parallaxFactor, 0f);
		}
		else
		{
			base.transform.position += new Vector3(vector.x * parallaxFactor, 0f, 0f);
		}
		lastCameraPosition = cameraTransform.position;
	}
}
