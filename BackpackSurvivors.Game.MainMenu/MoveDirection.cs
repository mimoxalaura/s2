using UnityEngine;

namespace BackpackSurvivors.Game.MainMenu;

public class MoveDirection : MonoBehaviour
{
	[SerializeField]
	private float _speed;

	[SerializeField]
	private Vector3 _direction;

	private void Update()
	{
		base.transform.Translate(_direction * Time.deltaTime * _speed);
	}
}
