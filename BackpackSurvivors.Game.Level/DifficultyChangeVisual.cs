using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class DifficultyChangeVisual : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Animator _animator;

	private bool _toggled;

	public void Toggle(bool toggled)
	{
		if (!_toggled && toggled)
		{
			_animator.SetBool("Activated", toggled);
		}
		_toggled = toggled;
	}
}
