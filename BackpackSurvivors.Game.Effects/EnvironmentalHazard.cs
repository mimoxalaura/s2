using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class EnvironmentalHazard : MonoBehaviour
{
	internal delegate void TriggerOnStayHandler(object sender, TriggerOnStayEventArgs e);

	[SerializeField]
	private TriggerOnStay TriggerOnStay;

	[SerializeField]
	private DamageSO _damageSO;

	[SerializeField]
	private float _knockbackPower;

	[SerializeField]
	private Enums.CharacterType _characterTypeForTriggerOnTouch;

	private DamageInstance _damageInstance;

	internal event TriggerOnStayHandler OnTriggerOnStay;

	private void Start()
	{
		_damageInstance = new DamageInstance(_damageSO);
		if (TriggerOnStay != null)
		{
			TriggerOnStay.OnTriggerOnStay += TriggerOnStay_OnTriggerOnStay;
		}
		if (TriggerOnStay != null)
		{
			TriggerOnStay.Init(_characterTypeForTriggerOnTouch);
		}
	}

	private void TriggerOnStay_OnTriggerOnStay(object sender, TriggerOnStayEventArgs e)
	{
		this.OnTriggerOnStay?.Invoke(this, e);
		if (SingletonController<GameController>.Instance.Player.CanTakeDamage())
		{
			bool wasCrit;
			float damage = DamageEngine.CalculateDamage(new Dictionary<Enums.WeaponStatType, float>(), new Dictionary<Enums.ItemStatType, float>(), SingletonController<GameController>.Instance.Player.CalculatedStats, _damageInstance, e.TriggeredOn.GetCharacterType(), out wasCrit);
			SingletonController<GameController>.Instance.Player.Damage(damage, wasCrit, base.gameObject, _knockbackPower, null, _damageInstance.CalculatedDamageType);
		}
	}

	private void OnDestroy()
	{
		if (TriggerOnStay != null)
		{
			TriggerOnStay.OnTriggerOnStay -= TriggerOnStay_OnTriggerOnStay;
		}
	}
}
