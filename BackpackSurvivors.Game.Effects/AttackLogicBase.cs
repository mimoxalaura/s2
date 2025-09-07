using System;
using System.Collections;
using BackpackSurvivors.Game.Effects.AttackLogic.WeaponSpecific;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class AttackLogicBase : MonoBehaviour
{
	[SerializeField]
	private AttackLogicStepSO[] _attackLogicSteps;

	private Collider2D _collider2D;

	internal Collider2D Collider2D => _collider2D;

	internal event Action OnReadyToScan;

	internal virtual IEnumerator RunAttackLogicAsync()
	{
		AttackLogicStepSO[] attackLogicSteps = _attackLogicSteps;
		foreach (AttackLogicStepSO attackLogicStep in attackLogicSteps)
		{
			yield return new WaitForSeconds(attackLogicStep.HitDelay);
			if (attackLogicStep.Rotate)
			{
				Collider2D.transform.rotation = attackLogicStep.PreferredColliderRotation;
			}
			if (attackLogicStep.Scale)
			{
				Collider2D.transform.localScale = attackLogicStep.PreferredColliderScale;
			}
			RaiseReadyToScan();
		}
	}

	public void StartAttackLogic(Collider2D collider2D)
	{
		_collider2D = collider2D;
		StartCoroutine(RunAttackLogicAsync());
	}

	internal void RaiseReadyToScan()
	{
		this.OnReadyToScan?.Invoke();
	}
}
