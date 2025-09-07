using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class WeaponAttackRotatingPoint : MonoBehaviour
{
	private Vector2 _targetPosition;

	public void SetTarget(Vector2 targetPosition)
	{
		_targetPosition = targetPosition;
	}

	private void Update()
	{
		Vector3 vector = base.transform.InverseTransformPoint(_targetPosition);
		float num = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		if (!double.IsNaN(0f - num))
		{
			base.transform.eulerAngles = new Vector3(0f, 0f, 0f - num);
		}
	}
}
