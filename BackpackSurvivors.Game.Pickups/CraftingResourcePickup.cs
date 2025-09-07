using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

internal class CraftingResourcePickup : Lootdrop
{
	[SerializeField]
	private Enums.CraftingResource _craftingResource;

	[SerializeField]
	private int _amount = 1;

	[SerializeField]
	private GameObject _beam;

	internal void SetDropAmount(int amount)
	{
		_amount = amount;
	}

	internal override void RemoveVisualAssetsOnMoving()
	{
		base.RemoveVisualAssetsOnMoving();
		_beam.SetActive(value: false);
	}

	protected override void Collect()
	{
		_collected = true;
		SingletonController<CurrencyController>.Instance.GainCraftingResource(_craftingResource, _amount, Enums.CurrencySource.Drop);
		SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup($"+ {_amount} {StringHelper.GetCleanString(_craftingResource)}", Constants.Colors.PositiveEffectColor, 2f);
		Object.Destroy(base.gameObject);
	}
}
