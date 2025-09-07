using System.ComponentModel;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Loot;

[CreateAssetMenu(fileName = "Loot", menuName = "Game/Items/Loot", order = 1)]
public class LootSO : ScriptableObject
{
	[SerializeField]
	internal Lootdrop Lootdrop;

	[SerializeField]
	internal Enums.LootType LootType;

	[Description("Between 0 and 100")]
	[SerializeField]
	internal float DropChance;

	[Description("The minimum amount to drop")]
	[SerializeField]
	internal int MinDropAmount;

	[Description("The maximum amount to drop")]
	[SerializeField]
	internal int MaxDropAmount;
}
