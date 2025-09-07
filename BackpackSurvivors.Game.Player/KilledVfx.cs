using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class KilledVfx : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	public void ShowVfx()
	{
		_animator.SetTrigger("Explode");
	}
}
