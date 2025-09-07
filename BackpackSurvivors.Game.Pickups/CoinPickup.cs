using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

internal class CoinPickup : Lootdrop
{
	internal delegate void CoinPickedUpHandler(object sender, CoinPickedUpEventArgs e);

	[SerializeField]
	private GameObject _bigGoldPileBeam;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private TrailRenderer _trailRenderer;

	private int _actualCoinValue;

	internal int CoinValue => _actualCoinValue;

	internal event CoinPickedUpHandler OnCoinPickedUp;

	internal override void AfterStart()
	{
		_bigGoldPileBeam.SetActive(_actualCoinValue > 20);
		SingletonController<EventController>.Instance.RegisterCoinPickupSpawned(this);
	}

	protected override void Collect()
	{
		_collected = true;
		if (_actualCoinValue > 20)
		{
			SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup($"+{_actualCoinValue} gold", Constants.Colors.CoinColor, 3f);
		}
		LootDropContainer component = base.transform.parent.GetComponent<LootDropContainer>();
		if (component != null)
		{
			component.RemoveDrop(Enums.LootType.Coins, this);
		}
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.Coins, _actualCoinValue, Enums.CurrencySource.Drop);
		TriggerPickUpEvent();
		Object.Destroy(base.gameObject);
	}

	internal void AddValue(int value)
	{
		_actualCoinValue += value;
		_spriteRenderer.sprite = SpriteHelper.GetCoinSprite(_actualCoinValue);
		base.gameObject.name = $"Stable Coin ({_actualCoinValue})";
		_bigGoldPileBeam.SetActive(_actualCoinValue > 20);
	}

	internal void UpdateValue(int value)
	{
		_actualCoinValue = value;
		_spriteRenderer.sprite = SpriteHelper.GetCoinSprite(_actualCoinValue);
		_bigGoldPileBeam.SetActive(_actualCoinValue > 20);
	}

	private void TriggerPickUpEvent()
	{
		CoinPickedUpEventArgs e = new CoinPickedUpEventArgs(base.transform.position);
		this.OnCoinPickedUp?.Invoke(this, e);
	}

	internal override void SetLayer(int layerId)
	{
		base.SetLayer(layerId);
		_spriteRenderer.sortingLayerID = layerId;
		_trailRenderer.sortingLayerID = layerId;
	}
}
