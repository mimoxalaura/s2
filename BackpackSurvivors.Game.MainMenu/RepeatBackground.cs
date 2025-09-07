using UnityEngine;

namespace BackpackSurvivors.Game.MainMenu;

public class RepeatBackground : MonoBehaviour
{
	private Vector3 _startPosition;

	private float _repeathWidth;

	[SerializeField]
	private bool _useX;

	private void Start()
	{
		_startPosition = base.transform.position;
		_repeathWidth = GetComponent<BoxCollider2D>().size.x;
	}

	private void Update()
	{
		if (_useX)
		{
			if (base.transform.localPosition.x < _startPosition.x - _repeathWidth)
			{
				base.transform.position = _startPosition;
			}
		}
		else if (base.transform.position.y > _startPosition.y + _repeathWidth)
		{
			base.transform.position = _startPosition;
		}
	}
}
