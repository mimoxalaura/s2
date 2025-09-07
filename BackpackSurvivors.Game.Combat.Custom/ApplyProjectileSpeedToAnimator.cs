using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Custom;

internal class ApplyProjectileSpeedToAnimator : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private string _projectileSpeedParameterName;

	private void Start()
	{
		float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.ProjectileSpeed);
		_animator.SetFloat(_projectileSpeedParameterName, calculatedStat);
	}
}
