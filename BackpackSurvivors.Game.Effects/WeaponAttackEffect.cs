using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class WeaponAttackEffect : MonoBehaviour
{
	[SerializeField]
	public Enums.WeaponAttackEffectTrigger WeaponAttackEffectTrigger;

	[SerializeField]
	public WeaponEffectHandler HandlerOnTrigger;

	[SerializeField]
	public string SimpleEffectDescription;

	[SerializeField]
	public int Id;
}
