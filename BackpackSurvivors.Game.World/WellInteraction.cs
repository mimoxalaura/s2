using System.Collections;
using BackpackSurvivors.Game.Core;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class WellInteraction : Interaction
{
	[SerializeField]
	public Animator _animator;

	private bool _wellIsUp;

	private bool dailyGamblingDone;

	public override void DoStart()
	{
		base.DoStart();
		dailyGamblingDone = false;
		CanInteract = !dailyGamblingDone && !GameDatabase.IsDemo;
	}

	internal void ResetCanInteract()
	{
		CanInteract = !dailyGamblingDone && !GameDatabase.IsDemo;
	}

	public override void DoInRange()
	{
		base.DoInRange();
	}

	public override void DoOutOfRange()
	{
		base.DoOutOfRange();
	}

	public override void DoInteract()
	{
		DoOutOfRange();
		dailyGamblingDone = true;
		CanInteract = false;
		_wellIsUp = !_wellIsUp;
		_animator.SetBool("Up", _wellIsUp);
		StartCoroutine(ShowRewardAfterDelay());
	}

	private IEnumerator ShowRewardAfterDelay()
	{
		yield return new WaitForSeconds(2f);
	}
}
