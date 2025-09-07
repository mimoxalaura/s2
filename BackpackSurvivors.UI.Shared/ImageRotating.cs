using UnityEngine;

namespace BackpackSurvivors.UI.Shared;

internal class ImageRotating : MonoBehaviour
{
	[SerializeField]
	private float _singleRotationTime = 1f;

	[SerializeField]
	private Vector3 _spinVector;

	[SerializeField]
	private bool _ignoreTimescaleZero;

	private void Start()
	{
		if (_ignoreTimescaleZero)
		{
			LeanTween.rotateAroundLocal(base.gameObject, _spinVector, 360f, _singleRotationTime).setRepeat(-1).setIgnoreTimeScale(useUnScaledTime: true);
		}
		else
		{
			LeanTween.rotateAroundLocal(base.gameObject, _spinVector, 360f, _singleRotationTime).setRepeat(-1);
		}
	}
}
