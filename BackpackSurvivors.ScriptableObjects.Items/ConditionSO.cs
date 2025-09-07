using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Condition", menuName = "Game/Items/Condition", order = 1)]
public class ConditionSO : ScriptableObject
{
	[SerializeField]
	public Enums.ConditionalStats.ConditionTarget ConditionTarget;

	[SerializeField]
	public Enums.ConditionalStats.ConditionCheckType ConditionCheckType;

	[SerializeField]
	public float CheckAmount;

	[SerializeField]
	public Enums.ConditionalStats.TypeToCheckAgainst TypeToCheckAgainst;

	[SerializeField]
	public Enums.PlaceableTagType PlaceableTagType;

	[SerializeField]
	public Enums.DamageType DamageTypeTag;

	[SerializeField]
	public Enums.WeaponType WeaponTypeTag;

	[SerializeField]
	public Enums.PlaceableWeaponSubtype WeaponSubtypeTag;

	[SerializeField]
	public Enums.PlaceableItemSubtype ItemSubtypeTag;

	[SerializeField]
	public Enums.PlaceableRarity PlaceableRarity;

	[SerializeField]
	public Enums.ItemStatType ItemStatType;

	[SerializeField]
	public Enums.PlaceableType PlaceableType;

	[SerializeField]
	public Enums.DamageType DamageType;

	[SerializeField]
	public Enums.WeaponType WeaponType;
}
