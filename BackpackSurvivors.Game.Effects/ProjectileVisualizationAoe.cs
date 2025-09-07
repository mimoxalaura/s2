using System.Collections;
using BackpackSurvivors.Game.Combat;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class ProjectileVisualizationAoe : ProjectileVisualization
{
	[SerializeField]
	private RaycastColliderHelper _raycastColliderHelper;

	[SerializeField]
	private AttackLogicBase AttackLogic;

	private AttackLogicBase _attackLogic;

	internal override void OverrideAwake()
	{
		if (AttackLogic != null)
		{
			_attackLogic = Object.Instantiate(AttackLogic, base.transform);
			_attackLogic.OnReadyToScan += AttackLogic_OnReadyToScan;
		}
	}

	private void AttackLogic_OnReadyToScan()
	{
		ActivateHitScan();
	}

	private void ActivateHitScan()
	{
		StartCoroutine(ActivateHitScanAsync());
	}

	private IEnumerator ActivateHitScanAsync()
	{
		int pauseFrameEveryXHits = 3;
		int counter = 0;
		if (_raycastColliderHelper.GetHitTargets(out var charactersHit))
		{
			foreach (Character item in charactersHit)
			{
				RaiseOnTriggerOnTouch(item);
				counter++;
				if (counter >= pauseFrameEveryXHits)
				{
					counter = 0;
					yield return 0;
				}
			}
		}
		yield return null;
	}

	internal override void OverrideDestroy()
	{
		base.OverrideDestroy();
		if (_attackLogic != null)
		{
			_attackLogic.OnReadyToScan -= AttackLogic_OnReadyToScan;
		}
	}

	internal override void StartAttackLogic()
	{
		if (!(_attackLogic == null))
		{
			_attackLogic.StartAttackLogic(_raycastColliderHelper.Collider2D);
		}
	}
}
