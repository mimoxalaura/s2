using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

[RequireComponent(typeof(Collider2D))]
internal class TriggerOnTouch : MonoBehaviour
{
	internal delegate void TriggerOnTouchHandler(object sender, TriggerOnTouchEventArgs e);

	internal delegate void TriggerOnStayHandler(object sender, TriggerOnStayEventArgs e);

	private Enums.CharacterType _damageTargetType;

	internal event TriggerOnTouchHandler OnTriggerOnTouch;

	internal void Init(Enums.CharacterType damageTargetType)
	{
		_damageTargetType = damageTargetType;
	}

	private void OnTriggerEnter2D(Collider2D collider2D)
	{
		HandleCollision(collider2D);
	}

	private void HandleCollision(Collider2D collider2D)
	{
		if (collider2D.TryGetComponent<Enemy>(out var component) && _damageTargetType == Enums.CharacterType.Enemy)
		{
			this.OnTriggerOnTouch?.Invoke(this, new TriggerOnTouchEventArgs(component));
		}
		if (collider2D.TryGetComponent<BackpackSurvivors.Game.Player.Player>(out var component2) && _damageTargetType == Enums.CharacterType.Player)
		{
			this.OnTriggerOnTouch?.Invoke(this, new TriggerOnTouchEventArgs(component2));
		}
	}
}
