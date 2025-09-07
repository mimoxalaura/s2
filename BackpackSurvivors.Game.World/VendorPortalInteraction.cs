using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.World;

public class VendorPortalInteraction : Interaction
{
	public override void DoStart()
	{
		base.DoStart();
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
		SingletonController<GameController>.Instance.Player.Despawn();
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: false);
		StartCoroutine(PortalAsync());
	}

	private IEnumerator PortalAsync()
	{
		yield return new WaitForSeconds(2f);
		SingletonController<GameController>.Instance.NextLevel();
	}
}
