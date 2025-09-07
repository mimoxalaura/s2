using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Custom;

internal class SpriteRotating : MonoBehaviour
{
	[SerializeField]
	private float _rotationSpeed = -1440f;

	[SerializeField]
	private float _rotationDuration = 4f;

	private void Start()
	{
		LeanTween.rotate(base.gameObject, new Vector3(0f, 0f, _rotationSpeed), _rotationDuration);
	}

	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
