using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups.Events;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Pickups;

internal class HealthPickup : Lootdrop
{
	internal delegate void HealthPickedUpHandler(object sender, HealthPickedUpEventArgs e);

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private TrailRenderer _trailRenderer;

	[SerializeField]
	private ParticleSystem _particleSystem;

	private float _healthValue = 0.1f;

	internal event HealthPickedUpHandler OnHealthPickedUp;

	private void Start()
	{
		SingletonController<EventController>.Instance.RegisterHealthPickupSpawned(this);
	}

	internal void SetHealthValue(int healthValue)
	{
		_healthValue = healthValue;
	}

	protected override bool CanCollect()
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		float health = player.HealthSystem.GetHealth();
		return player.HealthSystem.GetHealthMax() > health;
	}

	protected override void Collect()
	{
		_collected = true;
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		float num = player.HealthSystem.GetHealthMax() * _healthValue;
		player.HealthSystem.Heal(num);
		SingletonController<GameController>.Instance.Player.DamageVisualizer.ShowTextPopup($"<sprite name=Health> {(int)num}", Constants.Colors.PositiveEffectColor, 2f);
		TriggerPickUpEvent();
		Object.Destroy(base.gameObject);
	}

	private void TriggerPickUpEvent()
	{
		HealthPickedUpEventArgs e = new HealthPickedUpEventArgs(base.transform.position);
		this.OnHealthPickedUp?.Invoke(this, e);
	}

	internal override void SetLayer(int layerId)
	{
		base.SetLayer(layerId);
		_spriteRenderer.sortingLayerID = layerId;
		_trailRenderer.sortingLayerID = layerId;
		_particleSystem.GetComponent<Renderer>().sortingLayerID = layerId;
	}
}
