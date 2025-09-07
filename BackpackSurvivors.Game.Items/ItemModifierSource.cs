using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Items;

public class ItemModifierSource
{
	public Enums.ItemModifierSourceType WeaponModifierSourceType;

	public ItemInstance GlobalItem;

	public ItemInstance ConnectedItem;

	public ItemInstance ConditionalStatsItem;

	public WeaponInstance ConnectedWeapon;

	public RelicSO Relic;

	public TalentSO Talent;

	public BagSO Bag;

	public BuffSO BuffSO;
}
