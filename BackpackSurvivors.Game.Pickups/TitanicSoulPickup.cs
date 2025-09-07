using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

internal class TitanicSoulPickup : Lootdrop
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private TrailRenderer _trailRenderer;

	private int _titanicSoulValue = 1;

	internal void SetTitanicSoulValue(int coinValue)
	{
		_titanicSoulValue = coinValue;
	}

	internal override void SetLayer(int layerId)
	{
		base.SetLayer(layerId);
		_spriteRenderer.sortingLayerID = layerId;
		_trailRenderer.sortingLayerID = layerId;
	}

	protected override void Collect()
	{
		_collected = true;
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.TitanSouls, _titanicSoulValue, Enums.CurrencySource.Drop);
		SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup("+ Titan Souls", Constants.Colors.PositiveEffectColor, 3f, 0.8f);
		Object.Destroy(base.gameObject);
	}
}
