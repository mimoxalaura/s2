using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.Pickups;

[RequireComponent(typeof(CircleCollider2D))]
internal class PickupRadius : MonoBehaviour
{
	internal enum PickupRadiusType
	{
		MoveToPlayer,
		Collect
	}

	[SerializeField]
	private float _baseRadius;

	[SerializeField]
	private Light2D _light2d;

	[SerializeField]
	internal PickupRadiusType Type;

	private CircleCollider2D _circleCollider;

	internal float ActualRadius;

	internal void UpdatePickupRadius(float radiusModifier)
	{
		ActualRadius = Mathf.Clamp(_baseRadius * radiusModifier, 0f, 100f);
		_circleCollider.radius = ActualRadius;
		if (_light2d != null)
		{
			_light2d.pointLightOuterRadius = ActualRadius;
		}
	}

	private void Awake()
	{
		_circleCollider = GetComponent<CircleCollider2D>();
	}
}
