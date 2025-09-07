using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class EnemySpawningPortal : MonoBehaviour
{
	[SerializeField]
	private Animator _enemySpawningPortalAnimationController;

	public void Open()
	{
		_enemySpawningPortalAnimationController.SetBool("Closed", value: false);
	}

	public void Close()
	{
		_enemySpawningPortalAnimationController.SetBool("Closed", value: true);
	}
}
