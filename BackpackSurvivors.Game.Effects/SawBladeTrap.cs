using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class SawBladeTrap : MonoBehaviour
{
	[SerializeField]
	private Transform _startPoint;

	[SerializeField]
	private Transform _endPoint;

	[SerializeField]
	private float _moveDuration;

	private void Start()
	{
		LeanTween.move(base.gameObject, _endPoint.position, _moveDuration).setLoopPingPong();
	}
}
