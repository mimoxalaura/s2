using UnityEngine;

namespace IndieImpulseAssets;

public class RotateUI : MonoBehaviour
{
	private RectTransform rectTransform;

	public float rotationSpeed = 30f;

	public RotateType rotateType = RotateType.Z;

	private void Update()
	{
		rectTransform = GetComponent<RectTransform>();
		switch (rotateType)
		{
		case RotateType.X:
			RotateAxis(Vector3.right);
			break;
		case RotateType.Y:
			RotateAxis(Vector3.up);
			break;
		case RotateType.Z:
			RotateAxis(Vector3.forward);
			break;
		case RotateType.XYZ:
			RotateAxis(Vector3.up);
			RotateAxis(Vector3.right);
			RotateAxis(Vector3.forward);
			break;
		case RotateType.XZ:
			RotateAxis(Vector3.right);
			RotateAxis(Vector3.forward);
			break;
		}
	}

	private void RotateAxis(Vector3 axis)
	{
		float angle = rotationSpeed * Time.deltaTime;
		rectTransform.Rotate(axis, angle);
	}
}
