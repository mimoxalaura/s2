using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

[RequireComponent(typeof(Collider2D))]
public class TriggerOnStay : MonoBehaviour
{
	public delegate void TriggerOnStayHandler(object sender, TriggerOnStayEventArgs e);

	private Enums.CharacterType _damageTargetType;

	public event TriggerOnStayHandler OnTriggerOnStay;

	public void Init(Enums.CharacterType damageTargetType)
	{
		_damageTargetType = damageTargetType;
	}

	private void OnTriggerStay2D(Collider2D collider2D)
	{
		if (collider2D.TryGetComponent<Enemy>(out var component) && _damageTargetType == Enums.CharacterType.Enemy)
		{
			this.OnTriggerOnStay?.Invoke(this, new TriggerOnStayEventArgs(component));
		}
		if (collider2D.TryGetComponent<BackpackSurvivors.Game.Player.Player>(out var component2) && _damageTargetType == Enums.CharacterType.Player)
		{
			this.OnTriggerOnStay?.Invoke(this, new TriggerOnStayEventArgs(component2));
		}
	}
}
