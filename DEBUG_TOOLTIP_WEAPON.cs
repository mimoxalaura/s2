using System.Linq;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

public class DEBUG_TOOLTIP_WEAPON : MonoBehaviour
{
	[SerializeField]
	private WeaponSO weaponSO;

	[SerializeField]
	private WeaponTooltipTrigger weaponTooltipTrigger;

	private void Start()
	{
		WeaponInstance weaponInstance = new WeaponInstance(weaponSO);
		weaponInstance.ResetStats();
		weaponInstance.AddModifier(CreateModifier(Enums.WeaponStatType.ProjectileCount, 1f, SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableTalents.FirstOrDefault(), Enums.ItemModifierSourceType.Talent));
		weaponInstance.CalculateDebuffEffects();
		weaponTooltipTrigger.SetWeaponContent(weaponInstance, Enums.Backpack.DraggableOwner.Player);
	}

	private WeaponStatModifier CreateModifier(Enums.WeaponStatType weaponStatType, float value, object source, Enums.ItemModifierSourceType sourceType)
	{
		ItemModifierSource itemModifierSource = new ItemModifierSource();
		itemModifierSource.WeaponModifierSourceType = sourceType;
		switch (sourceType)
		{
		case Enums.ItemModifierSourceType.Talent:
			itemModifierSource.Talent = (TalentSO)source;
			break;
		case Enums.ItemModifierSourceType.ConnectedItem:
			itemModifierSource.ConnectedItem = new ItemInstance((ItemSO)source);
			break;
		case Enums.ItemModifierSourceType.GlobalItem:
			itemModifierSource.GlobalItem = new ItemInstance((ItemSO)source);
			break;
		case Enums.ItemModifierSourceType.Relic:
			itemModifierSource.Relic = (RelicSO)source;
			break;
		}
		return new WeaponStatModifier
		{
			CalculatedBonus = value,
			WeaponStatType = weaponStatType,
			Source = itemModifierSource
		};
	}
}
