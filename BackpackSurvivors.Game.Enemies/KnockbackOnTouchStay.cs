using BackpackSurvivors.Game.Player;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

[RequireComponent(typeof(Collider2D))]
public class KnockbackOnTouchStay : MonoBehaviour
{
	[SerializeField]
	private float _knockbackPower;

	[SerializeField]
	private GameObject _knockbackSource;

	[SerializeField]
	private Enums.CharacterType _characterTargetType;

	public bool EnableKnockback;

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!EnableKnockback)
		{
			return;
		}
		if (_characterTargetType == Enums.CharacterType.Player)
		{
			BackpackSurvivors.Game.Player.Player component = collision.gameObject.GetComponent<BackpackSurvivors.Game.Player.Player>();
			if (component == null)
			{
				return;
			}
			component.Knockback(_knockbackSource.transform, _knockbackPower);
		}
		if (_characterTargetType == Enums.CharacterType.Enemy)
		{
			Enemy component2 = collision.gameObject.GetComponent<Enemy>();
			if (!(component2 == null))
			{
				component2.Knockback(_knockbackSource.transform, _knockbackPower);
			}
		}
	}
}
