using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class StayInsideSquare : MonoBehaviour
{
	private Transform _minimapCam;

	private float _minimapSize;

	private Vector3 TempV3;

	private bool _active;

	public void EnableClamping(bool enabled)
	{
		_active = enabled;
	}

	public void Init(Transform minimapCam, float minimapSize)
	{
		_minimapCam = minimapCam;
		_minimapSize = minimapSize;
	}

	private void Update()
	{
		if (_active)
		{
			TempV3 = base.transform.parent.transform.position;
			TempV3.z = base.transform.position.z;
			base.transform.position = TempV3;
		}
	}

	private void LateUpdate()
	{
		if (_active && !(_minimapCam == null))
		{
			base.transform.position = new Vector3(Mathf.Clamp(base.transform.position.x, _minimapCam.position.x - _minimapSize, _minimapSize + _minimapCam.position.x), Mathf.Clamp(base.transform.position.y, _minimapCam.position.y - _minimapSize, _minimapSize + _minimapCam.position.y), base.transform.position.z);
		}
	}
}
