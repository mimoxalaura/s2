using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class StayInsideRadius : MonoBehaviour
{
	private Transform _centerTransform;

	private float _maxRadius;

	private Vector3 TempV3;

	private bool _active;

	public void EnableClamping(bool enabled)
	{
		_active = enabled;
	}

	public void Init(Transform centerTransform, float maxRadius)
	{
		_centerTransform = centerTransform;
		_maxRadius = maxRadius;
	}

	private void Update()
	{
		_ = _active;
	}

	private void LateUpdate()
	{
		if (_active && !(_centerTransform == null) && !SingletonController<GameController>.Instance.GamePaused)
		{
			Vector3 position = _centerTransform.position;
			position.y -= 0.5f;
			float num = Vector3.Distance(base.transform.position, position);
			if (num > _maxRadius)
			{
				Vector3 vector = base.transform.position - position;
				vector *= _maxRadius / num;
				base.transform.position = position + vector;
			}
		}
	}
}
