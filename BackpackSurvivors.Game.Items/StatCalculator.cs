using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Combat.ScriptableObjects;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Stats;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items;

public class StatCalculator
{
	public static event Action OnStatsRecalulated;

	public static void RecalculateStats(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> basePlayerStats, List<WeaponInstance> weaponsInBackpack, List<ItemInstance> itemsInBackpack, out Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStatsOut, out Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> damageTypeValues, List<RelicSO> activeRelics, List<TalentSO> activeTalents, Dictionary<WeaponInstance, List<ItemInstance>> weaponsAndItemStarSources, Dictionary<WeaponInstance, List<WeaponInstance>> weaponsAndWeaponStarSources, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags, Dictionary<Enums.ItemStatType, float> playerStatBuffs, ICountController countController, Dictionary<Enums.DamageType, float> baseDamageTypeValues, List<WeaponInstance> weaponsInStorage, List<ItemInstance> itemsInStorage, List<WeaponInstance> weaponsInShop, List<ItemInstance> itemsInShop)
	{
		ResetWeaponStats(weaponsInBackpack);
		ResetWeaponStats(weaponsInShop);
		ResetWeaponStats(weaponsInStorage);
		damageTypeValues = CreateComplexDamageTypeDictionary();
		playerStatsOut = FillPlayerStatsFromBase(basePlayerStats);
		Dictionary<Enums.ItemStatType, List<ItemStatModifier>> basePlayerStats2 = FillPlayerStatsFromBase(basePlayerStats);
		CalculateItemFormulaStats(itemsInBackpack);
		CalculateItemFormulaStats(itemsInStorage);
		CalculateItemFormulaStats(itemsInShop);
		CalculateItemFormulaDamageTypeValues(itemsInBackpack);
		CalculateItemFormulaDamageTypeValues(itemsInStorage);
		CalculateItemFormulaDamageTypeValues(itemsInShop);
		List<ItemStatModifier> itemStatModifiers = CalculateItemStatModifiersForPlayer(itemsInBackpack, activeRelics, activeTalents);
		List<ItemStatModifier> itemStatModifiers2 = CalculateConditionalItemStatModifiersForPlayer(itemsInBackpack, activeRelics, activeTalents);
		List<DamageTypeValueModifier> damageTypeValueModifiers = CalculateDamageTypeValueModifiersFromGlobalAspectsForPlayer(itemsInBackpack, weaponsInBackpack, activeRelics, activeTalents, weaponsAndItemStarSources, weaponsAndWeaponStarSources, weaponsAndContainingBags, playerStatBuffs, baseDamageTypeValues);
		SetPlayerStatsFromModifiers(itemStatModifiers, playerStatsOut);
		SetPlayerStatsFromModifiers(itemStatModifiers2, playerStatsOut);
		SetPlayerDamageValuesFromModifiers(damageTypeValueModifiers, damageTypeValues);
		SetPlayerStatsFromBuffs(playerStatBuffs, playerStatsOut);
		SetDamageOverrides(itemsInBackpack, weaponsInBackpack, activeRelics, activeTalents, weaponsAndItemStarSources);
		countController.UpdateCounts();
		List<WeaponStatModifier> baseWeaponStatModifiers = CalculateStatModifiersForWeapons(basePlayerStats2, weaponsInBackpack, weaponsInShop, weaponsInStorage, itemsInBackpack, activeRelics, activeTalents, weaponsAndItemStarSources, weaponsAndWeaponStarSources, weaponsAndContainingBags, playerStatBuffs);
		List<WeaponAttackEffectModifier> weaponAttackEffectModifiers = CalculateWeaponAttackEffectModifiersForWeapons(weaponsInBackpack, itemsInBackpack, activeRelics, activeTalents, weaponsAndItemStarSources, weaponsAndWeaponStarSources, weaponsAndContainingBags);
		List<WeaponDebuffModifier> weaponDebuffModifiers = CalculateWeaponDebuffModifiersForWeapons(weaponsInBackpack, itemsInBackpack, activeRelics, activeTalents, weaponsAndItemStarSources, weaponsAndWeaponStarSources, weaponsAndContainingBags);
		ApplyStatModifiersToWeapons(weaponsInBackpack, baseWeaponStatModifiers);
		ApplyStatModifiersToWeapons(weaponsInShop, baseWeaponStatModifiers);
		ApplyStatModifiersToWeapons(weaponsInStorage, baseWeaponStatModifiers);
		ApplyWeaponAttackEffectsToWeapons(weaponsInBackpack, weaponAttackEffectModifiers);
		ApplyWeaponAttackEffectsToWeapons(weaponsInShop, weaponAttackEffectModifiers);
		ApplyWeaponAttackEffectsToWeapons(weaponsInStorage, weaponAttackEffectModifiers);
		AddWeaponDebuffsToWeapons(weaponsInBackpack, weaponDebuffModifiers);
		AddWeaponDebuffsToWeapons(weaponsInShop, weaponDebuffModifiers);
		AddWeaponDebuffsToWeapons(weaponsInStorage, weaponDebuffModifiers);
		CalculateWeaponStats(weaponsInBackpack);
		CalculateWeaponStats(weaponsInShop);
		CalculateWeaponStats(weaponsInStorage);
		SetModifiersFromOverrides(weaponsInBackpack, playerStatsOut);
		SetModifiersFromOverrides(weaponsInShop, playerStatsOut);
		SetModifiersFromOverrides(weaponsInStorage, playerStatsOut);
		SetDamageFromOverrides(weaponsInBackpack, playerStatsOut);
		SetDamageFromOverrides(weaponsInShop, playerStatsOut);
		SetDamageFromOverrides(weaponsInStorage, playerStatsOut);
		ApplyDamageModifiersToWeapons(weaponsInBackpack, damageTypeValueModifiers);
		ApplyDamageModifiersToWeapons(weaponsInShop, damageTypeValueModifiers);
		ApplyDamageModifiersToWeapons(weaponsInStorage, damageTypeValueModifiers);
		StatCalculator.OnStatsRecalulated?.Invoke();
	}

	private static void CalculateItemFormulaStats(List<ItemInstance> itemsInBackpack)
	{
		foreach (ItemInstance item in itemsInBackpack)
		{
			if (item.ItemSO.GlobalStats.FormulaStats == null || !item.ItemSO.GlobalStats.FormulaStats.Any())
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> formulaStat in item.ItemSO.GlobalStats.FormulaStats)
			{
				Enums.ItemStatType key = formulaStat.Key;
				float statValue = MathF.Floor(formulaStat.Value.GetFormulaResult());
				item.SetCalculatedStat(key, statValue);
			}
		}
	}

	private static void CalculateItemFormulaDamageTypeValues(List<ItemInstance> itemsInBackpack)
	{
		foreach (ItemInstance item in itemsInBackpack)
		{
			if (item.ItemSO.GlobalStats.FormulaDamageTypeValues == null || !item.ItemSO.GlobalStats.FormulaDamageTypeValues.Any())
			{
				continue;
			}
			foreach (KeyValuePair<Enums.DamageType, FormulaSO> formulaDamageTypeValue in item.ItemSO.GlobalStats.FormulaDamageTypeValues)
			{
				Enums.DamageType key = formulaDamageTypeValue.Key;
				float formulaResult = formulaDamageTypeValue.Value.GetFormulaResult();
				item.SetCalculatedDamageTypeValue(key, formulaResult);
			}
		}
	}

	private static List<WeaponStatModifier> CalculateWeaponFormulaStats(List<WeaponInstance> weaponsInBackpack)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (item.BaseWeaponSO.Stats.FormulaStats == null || !item.BaseWeaponSO.Stats.FormulaStats.Any())
			{
				continue;
			}
			foreach (KeyValuePair<Enums.WeaponStatType, FormulaSO> formulaStat in item.BaseWeaponSO.Stats.FormulaStats)
			{
				Enums.WeaponStatType key = formulaStat.Key;
				float formulaResult = formulaStat.Value.GetFormulaResult();
				WeaponStatModifier weaponStatModifier = new WeaponStatModifier();
				weaponStatModifier.WeaponStatType = key;
				weaponStatModifier.CalculatedBonus = formulaResult;
				ItemModifierFilter itemModifierFilter = new ItemModifierFilter
				{
					HardTargetWeaponInstance = item
				};
				weaponStatModifier.ItemModifierFilter = itemModifierFilter;
				ItemModifierSource itemModifierSource = new ItemModifierSource();
				itemModifierSource.WeaponModifierSourceType = Enums.ItemModifierSourceType.Formula;
				weaponStatModifier.Source = itemModifierSource;
				list.Add(weaponStatModifier);
			}
		}
		return list;
	}

	private static void CalculateWeaponStats(List<WeaponInstance> weapons)
	{
		foreach (WeaponInstance weapon in weapons)
		{
			weapon.CalculateStats();
		}
	}

	private static void SetPlayerStatsFromBuffs(Dictionary<Enums.ItemStatType, float> playerStatBuffs, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			if (playerStatBuffs.ContainsKey(value))
			{
				playerStats[value].Add(new ItemStatModifier
				{
					CalculatedBonus = playerStatBuffs[value],
					Source = new ItemModifierSource
					{
						WeaponModifierSourceType = Enums.ItemModifierSourceType.Buff
					},
					ItemStatType = value
				});
			}
		}
	}

	private static Dictionary<Enums.ItemStatType, List<ItemStatModifier>> FillPlayerStatsFromBase(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> basePlayerStats)
	{
		Dictionary<Enums.ItemStatType, List<ItemStatModifier>> dictionary = CreateComplexItemStatDictionary();
		if (basePlayerStats == null)
		{
			return dictionary;
		}
		foreach (KeyValuePair<Enums.ItemStatType, List<ItemStatModifier>> basePlayerStat in basePlayerStats)
		{
			if (dictionary.ContainsKey(basePlayerStat.Key))
			{
				dictionary[basePlayerStat.Key] = new List<ItemStatModifier>();
				dictionary[basePlayerStat.Key].AddRange(basePlayerStats[basePlayerStat.Key]);
			}
		}
		return dictionary;
	}

	private static List<ItemStatModifier> CalculateItemStatModifiersForPlayer(List<ItemInstance> itemsInBackpack, List<RelicSO> activeRelics, List<TalentSO> activeTalentSOs)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		list.AddRange(LoadItemStatModifiersFromRelics(activeRelics));
		list.AddRange(LoadItemStatModifiersFromGlobalItems(itemsInBackpack));
		list.AddRange(LoadItemStatModifiersFromBagPassives());
		list.AddRange(LoadItemStatModifiersFromTalents(activeTalentSOs));
		return list;
	}

	private static List<ItemStatModifier> CalculateConditionalItemStatModifiersForPlayer(List<ItemInstance> itemsInBackpack, List<RelicSO> activeRelics, List<TalentSO> activeTalents)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		list.AddRange(LoadConditionalItemStatModifiersFromRelics(activeRelics));
		list.AddRange(LoadConditionalItemStatModifiersFromTalents(activeTalents));
		list.AddRange(LoadConditionalItemStatModifiersFromConditionallItems(itemsInBackpack));
		list.AddRange(LoadConditionalItemStatModifiersFromBagPassives());
		return list;
	}

	private static IEnumerable<ItemStatModifier> LoadConditionalItemStatModifiersFromRelics(List<RelicSO> relics)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (RelicSO item2 in relics.Where((RelicSO r) => r.ConditionalStats != null))
		{
			ItemStatsSO[] conditionalStats = item2.ConditionalStats;
			foreach (ItemStatsSO itemStatsSO in conditionalStats)
			{
				if (itemStatsSO == null || GetConditionsFromSOs(itemStatsSO.Conditions).Any((Condition c) => !c.IsConditionSatisfied()))
				{
					continue;
				}
				foreach (KeyValuePair<Enums.ItemStatType, float> statValue in itemStatsSO.StatValues)
				{
					ItemStatModifier item = new ItemStatModifier
					{
						CalculatedBonus = statValue.Value,
						Source = new ItemModifierSource
						{
							Relic = item2,
							WeaponModifierSourceType = Enums.ItemModifierSourceType.Relic
						},
						ItemStatType = statValue.Key
					};
					list.Add(item);
				}
			}
		}
		return list;
	}

	private static IEnumerable<ItemStatModifier> LoadConditionalItemStatModifiersFromTalents(List<TalentSO> talents)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (TalentSO item2 in talents.Where((TalentSO t) => t.Stats != null))
		{
			if (GetConditionsFromSOs(item2.Stats.Conditions).Any((Condition c) => !c.IsConditionSatisfied()))
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, float> statValue in item2.Stats.StatValues)
			{
				ItemStatModifier item = new ItemStatModifier
				{
					CalculatedBonus = statValue.Value,
					Source = new ItemModifierSource
					{
						Talent = item2,
						WeaponModifierSourceType = Enums.ItemModifierSourceType.Talent
					},
					ItemStatType = statValue.Key
				};
				list.Add(item);
			}
		}
		return list;
	}

	private static IEnumerable<ItemStatModifier> LoadConditionalItemStatModifiersFromConditionallItems(List<ItemInstance> itemsInBackpack)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (ItemInstance item3 in itemsInBackpack.Where((ItemInstance i) => i.ConditionalStatConditions != null))
		{
			if (GetConditionsFromSOs(item3.ConditionalStatConditions).Any((Condition c) => !c.IsConditionSatisfied()))
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, float> conditionalStatValue in item3.ConditionalStatValues)
			{
				ItemStatModifier item = new ItemStatModifier
				{
					CalculatedBonus = conditionalStatValue.Value,
					Source = new ItemModifierSource
					{
						ConditionalStatsItem = item3,
						WeaponModifierSourceType = Enums.ItemModifierSourceType.ConditionalStatsItem
					},
					ItemStatType = conditionalStatValue.Key
				};
				list.Add(item);
			}
		}
		foreach (ItemInstance item4 in itemsInBackpack.Where((ItemInstance i) => i.ConditionalFormulaStatValues != null))
		{
			foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> conditionalFormulaStatValue in item4.ConditionalFormulaStatValues)
			{
				if (!GetConditionsFromSOs(item4.ConditionalStatConditions).Any((Condition c) => !c.IsConditionSatisfied()))
				{
					ItemStatModifier item2 = new ItemStatModifier
					{
						CalculatedBonus = conditionalFormulaStatValue.Value.GetFormulaResult(),
						Source = new ItemModifierSource
						{
							ConditionalStatsItem = item4,
							WeaponModifierSourceType = Enums.ItemModifierSourceType.ConditionalFormula
						},
						ItemStatType = conditionalFormulaStatValue.Key
					};
					list.Add(item2);
				}
			}
		}
		return list;
	}

	private static IEnumerable<ItemStatModifier> LoadConditionalItemStatModifiersFromBagPassives()
	{
		return new List<ItemStatModifier>();
	}

	private static List<ItemStatModifier> LoadItemStatModifiersFromRelics(List<RelicSO> relics)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (RelicSO item2 in relics.Where((RelicSO x) => x.RelicHandler))
		{
			if (item2.GlobalStats == null || item2.Conditions.Any((ConditionSO c) => c.ConditionTarget == Enums.ConditionalStats.ConditionTarget.Weapon))
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, float> statValue in item2.GlobalStats.StatValues)
			{
				ItemStatModifier item = new ItemStatModifier
				{
					CalculatedBonus = statValue.Value,
					Source = new ItemModifierSource
					{
						Relic = item2,
						WeaponModifierSourceType = Enums.ItemModifierSourceType.Relic
					},
					ItemStatType = statValue.Key
				};
				list.Add(item);
			}
		}
		return list;
	}

	private static List<ItemStatModifier> LoadItemStatModifiersFromGlobalItems(List<ItemInstance> items)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (ItemInstance item2 in items)
		{
			foreach (KeyValuePair<Enums.ItemStatType, float> calculatedStat in item2.CalculatedStats)
			{
				if (calculatedStat.Value != 0f)
				{
					ItemStatModifier item = new ItemStatModifier
					{
						CalculatedBonus = calculatedStat.Value,
						Source = new ItemModifierSource
						{
							GlobalItem = item2,
							WeaponModifierSourceType = Enums.ItemModifierSourceType.GlobalItem
						},
						ItemStatType = calculatedStat.Key
					};
					list.Add(item);
				}
			}
		}
		return list;
	}

	private static List<ItemStatModifier> LoadItemStatModifiersFromTalents(List<TalentSO> activeTalentSOs)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (TalentSO activeTalentSO in activeTalentSOs)
		{
			if (activeTalentSO?.Stats == null || activeTalentSO.Stats.Conditions != null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, float> statValue in activeTalentSO.Stats.StatValues)
			{
				ItemStatModifier item = new ItemStatModifier
				{
					CalculatedBonus = statValue.Value,
					Source = new ItemModifierSource
					{
						Talent = activeTalentSO,
						WeaponModifierSourceType = Enums.ItemModifierSourceType.Talent
					},
					ItemStatType = statValue.Key
				};
				list.Add(item);
			}
			foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> formulaStat in activeTalentSO.Stats.FormulaStats)
			{
				ItemStatModifier item2 = new ItemStatModifier
				{
					CalculatedBonus = formulaStat.Value.GetFormulaResult(),
					Source = new ItemModifierSource
					{
						Talent = activeTalentSO,
						WeaponModifierSourceType = Enums.ItemModifierSourceType.Talent
					},
					ItemStatType = formulaStat.Key
				};
				list.Add(item2);
			}
		}
		return list;
	}

	private static List<ItemStatModifier> LoadItemStatModifiersFromBagPassives()
	{
		return new List<ItemStatModifier>();
	}

	private static List<ItemStatModifier> LoadItemStatModifiersFromBags(List<ItemInstance> items)
	{
		List<ItemStatModifier> list = new List<ItemStatModifier>();
		foreach (ItemInstance item2 in items)
		{
			_ = item2;
			foreach (BagSO item3 in new List<BagSO>())
			{
				foreach (ItemStatModifier item4 in new List<ItemStatModifier>())
				{
					_ = item4;
					foreach (KeyValuePair<Enums.ItemStatType, float> statValue in item3.Stats.StatValues)
					{
						if (item3.Stats.Conditions.Any())
						{
							ItemStatModifier itemStatModifier = new ItemStatModifier
							{
								Source = new ItemModifierSource
								{
									WeaponModifierSourceType = Enums.ItemModifierSourceType.Bag,
									Bag = item3
								},
								ItemStatType = statValue.Key,
								CalculatedBonus = statValue.Value
							};
							ItemModifierFilter itemModifierFilter = new ItemModifierFilter
							{
								Conditions = GetConditionsFromSOs(item3.Stats.Conditions)
							};
							itemStatModifier.ItemModifierFilter = itemModifierFilter;
							list.Add(itemStatModifier);
						}
						else
						{
							ItemStatModifier item = new ItemStatModifier
							{
								Source = new ItemModifierSource
								{
									WeaponModifierSourceType = Enums.ItemModifierSourceType.Bag,
									Bag = item3
								},
								ItemStatType = statValue.Key,
								CalculatedBonus = statValue.Value
							};
							list.Add(item);
						}
					}
				}
			}
		}
		return list;
	}

	private static void SetPlayerStatsFromModifiers(List<ItemStatModifier> itemStatModifiers, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		foreach (Enums.ItemStatType itemStatType in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			foreach (ItemStatModifier item in itemStatModifiers.Where((ItemStatModifier x) => x.ItemStatType == itemStatType))
			{
				playerStats[itemStatType].Add(item);
			}
		}
	}

	private static List<DamageTypeValueModifier> CalculateDamageTypeValueModifiersFromGlobalAspectsForPlayer(List<ItemInstance> itemsInBackpack, List<WeaponInstance> weaponsInBackpack, List<RelicSO> activeRelics, List<TalentSO> activeTalents, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstance, Dictionary<WeaponInstance, List<WeaponInstance>> linkedWeaponsAndWeaponInstances, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags, Dictionary<Enums.ItemStatType, float> buffStats, Dictionary<Enums.DamageType, float> baseDamageTypeValues)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		list.AddRange(LoadDamageTypeModifiersFromCharacter(baseDamageTypeValues));
		list.AddRange(LoadDamageTypeValueModifiersFromRelics(activeRelics));
		list.AddRange(LoadDamageTypeValueModifiersFromTalents(activeTalents));
		list.AddRange(LoadDamageTypeValueModifiersFromGlobalItems(itemsInBackpack));
		list.AddRange(LoadDamageTypeValueModifiersFromStarredItems(weaponsInBackpack, linkedWeaponsAndItemInstance));
		list.AddRange(LoadDamageTypeValueModifiersFromStarredWeapons(weaponsInBackpack, linkedWeaponsAndWeaponInstances));
		list.AddRange(LoadDamageTypeModifiersFromConditionalStatsInItems(itemsInBackpack));
		list.AddRange(LoadDamageTypeValueModifiersFromBags(weaponsInBackpack, weaponsAndContainingBags));
		list.AddRange(LoadDamageTypeValueModifiersFromBuffStats(weaponsInBackpack, buffStats));
		return list;
	}

	private static IEnumerable<DamageTypeValueModifier> LoadDamageTypeModifiersFromConditionalStatsInItems(List<ItemInstance> itemsInBackpack)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (ItemInstance item in itemsInBackpack)
		{
			if (GetConditionsFromSOs(item.ConditionalStatConditions).Any((Condition c) => !c.IsConditionSatisfied()))
			{
				continue;
			}
			foreach (KeyValuePair<Enums.DamageType, float> conditionalDamageTypeValue in item.ConditionalDamageTypeValues)
			{
				if (conditionalDamageTypeValue.Value != 0f)
				{
					DamageTypeValueModifier damageTypeValueModifier = new DamageTypeValueModifier
					{
						Source = new ItemModifierSource
						{
							WeaponModifierSourceType = Enums.ItemModifierSourceType.ConditionalStatsItem,
							ConditionalStatsItem = item
						},
						DamageType = conditionalDamageTypeValue.Key,
						CalculatedBonus = conditionalDamageTypeValue.Value
					};
					ItemModifierFilter itemModifierFilter = new ItemModifierFilter
					{
						Conditions = GetConditionsFromSOs(item.ConditionalStatConditions)
					};
					damageTypeValueModifier.ItemModifierFilter = itemModifierFilter;
					list.Add(damageTypeValueModifier);
				}
			}
			if (item.ConditionalFormulaDamageTypeValues == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.DamageType, FormulaSO> conditionalFormulaDamageTypeValue in item.ConditionalFormulaDamageTypeValues)
			{
				Enums.DamageType key = conditionalFormulaDamageTypeValue.Key;
				float formulaResult = conditionalFormulaDamageTypeValue.Value.GetFormulaResult();
				if (formulaResult != 0f)
				{
					DamageTypeValueModifier damageTypeValueModifier2 = new DamageTypeValueModifier
					{
						Source = new ItemModifierSource
						{
							WeaponModifierSourceType = Enums.ItemModifierSourceType.ConditionalStatsItem,
							ConditionalStatsItem = item
						},
						DamageType = key,
						CalculatedBonus = formulaResult
					};
					ItemModifierFilter itemModifierFilter2 = new ItemModifierFilter
					{
						Conditions = GetConditionsFromSOs(item.ConditionalStatConditions)
					};
					damageTypeValueModifier2.ItemModifierFilter = itemModifierFilter2;
					list.Add(damageTypeValueModifier2);
				}
			}
		}
		return list;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromGlobalItems(List<ItemInstance> items)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (ItemInstance item2 in items)
		{
			foreach (KeyValuePair<Enums.DamageType, float> calculatedDamageTypeValue in item2.CalculatedDamageTypeValues)
			{
				if (calculatedDamageTypeValue.Value != 0f)
				{
					DamageTypeValueModifier item = new DamageTypeValueModifier
					{
						CalculatedBonus = calculatedDamageTypeValue.Value,
						Source = new ItemModifierSource
						{
							GlobalItem = item2,
							WeaponModifierSourceType = Enums.ItemModifierSourceType.GlobalItem
						},
						DamageType = calculatedDamageTypeValue.Key
					};
					list.Add(item);
				}
			}
		}
		return list;
	}

	private static IEnumerable<DamageTypeValueModifier> LoadDamageTypeModifiersFromCharacter(Dictionary<Enums.DamageType, float> baseCharacterDamageTypeValues)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (KeyValuePair<Enums.DamageType, float> baseCharacterDamageTypeValue in baseCharacterDamageTypeValues)
		{
			if (baseCharacterDamageTypeValue.Value != 0f)
			{
				DamageTypeValueModifier item = new DamageTypeValueModifier
				{
					CalculatedBonus = baseCharacterDamageTypeValue.Value,
					Source = new ItemModifierSource
					{
						WeaponModifierSourceType = Enums.ItemModifierSourceType.BaseCharacter
					},
					DamageType = baseCharacterDamageTypeValue.Key
				};
				list.Add(item);
			}
		}
		return list;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromRelics(List<RelicSO> relics)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (RelicSO relic in relics)
		{
			if (relic.GlobalStats == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.DamageType, float> damageTypeValue in relic.GlobalStats.DamageTypeValues)
			{
				if (damageTypeValue.Value != 0f)
				{
					DamageTypeValueModifier item = new DamageTypeValueModifier
					{
						CalculatedBonus = damageTypeValue.Value,
						Source = new ItemModifierSource
						{
							Relic = relic,
							WeaponModifierSourceType = Enums.ItemModifierSourceType.Relic
						},
						DamageType = damageTypeValue.Key
					};
					list.Add(item);
				}
			}
		}
		return list;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromTalents(List<TalentSO> talentSOs)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (TalentSO talentSO in talentSOs)
		{
			if (talentSO.Stats == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.DamageType, float> item2 in GetDamageTypeValueModifiersFromTalent(talentSO))
			{
				if (item2.Value != 0f)
				{
					DamageTypeValueModifier item = new DamageTypeValueModifier
					{
						CalculatedBonus = item2.Value,
						Source = new ItemModifierSource
						{
							Talent = talentSO,
							WeaponModifierSourceType = Enums.ItemModifierSourceType.Talent
						},
						DamageType = item2.Key
					};
					list.Add(item);
				}
			}
		}
		return list;
	}

	private static Dictionary<Enums.DamageType, float> GetDamageTypeValueModifiersFromTalent(TalentSO talentSO)
	{
		Dictionary<Enums.DamageType, float> dictionary = new Dictionary<Enums.DamageType, float>();
		foreach (KeyValuePair<Enums.DamageType, float> damageTypeValue in talentSO.Stats.DamageTypeValues)
		{
			if (!dictionary.ContainsKey(damageTypeValue.Key))
			{
				dictionary.Add(damageTypeValue.Key, 0f);
			}
			dictionary[damageTypeValue.Key] += damageTypeValue.Value;
		}
		foreach (KeyValuePair<Enums.DamageType, FormulaSO> formulaDamageTypeValue in talentSO.Stats.FormulaDamageTypeValues)
		{
			if (!dictionary.ContainsKey(formulaDamageTypeValue.Key))
			{
				dictionary.Add(formulaDamageTypeValue.Key, 0f);
			}
			float formulaResult = formulaDamageTypeValue.Value.GetFormulaResult();
			dictionary[formulaDamageTypeValue.Key] += formulaResult;
		}
		return dictionary;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromStarredItems(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstanceSets)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!linkedWeaponsAndItemInstanceSets.ContainsKey(item))
			{
				continue;
			}
			foreach (ItemInstance item2 in linkedWeaponsAndItemInstanceSets[item])
			{
				foreach (KeyValuePair<Enums.DamageType, float> starDamageTypeValue in item2.StarDamageTypeValues)
				{
					if (starDamageTypeValue.Value != 0f)
					{
						if (item2.StarWeaponConditions != null && item2.StarWeaponConditions.Any())
						{
							DamageTypeValueModifier damageTypeValueModifier = new DamageTypeValueModifier
							{
								Source = new ItemModifierSource
								{
									WeaponModifierSourceType = Enums.ItemModifierSourceType.ConnectedItem,
									ConnectedItem = item2
								},
								DamageType = starDamageTypeValue.Key,
								CalculatedBonus = starDamageTypeValue.Value
							};
							ItemModifierFilter itemModifierFilter = new ItemModifierFilter
							{
								Conditions = GetConditionsFromSOs(item2.StarWeaponConditions),
								HardTargetWeaponInstance = item
							};
							damageTypeValueModifier.ItemModifierFilter = itemModifierFilter;
							list.Add(damageTypeValueModifier);
						}
						else
						{
							DamageTypeValueModifier damageTypeValueModifier2 = new DamageTypeValueModifier
							{
								Source = new ItemModifierSource
								{
									WeaponModifierSourceType = Enums.ItemModifierSourceType.ConnectedItem,
									ConnectedItem = item2
								},
								DamageType = starDamageTypeValue.Key,
								CalculatedBonus = starDamageTypeValue.Value
							};
							ItemModifierFilter itemModifierFilter2 = new ItemModifierFilter
							{
								HardTargetWeaponInstance = item
							};
							damageTypeValueModifier2.ItemModifierFilter = itemModifierFilter2;
							list.Add(damageTypeValueModifier2);
						}
					}
				}
			}
		}
		return list;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromStarredWeapons(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<WeaponInstance>> linkedWeaponsAndWeaponInstanceSets)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!linkedWeaponsAndWeaponInstanceSets.ContainsKey(item))
			{
				continue;
			}
			foreach (WeaponInstance item2 in linkedWeaponsAndWeaponInstanceSets[item])
			{
				foreach (KeyValuePair<Enums.DamageType, float> baseStarDamageTypeValue in item2.BaseStarDamageTypeValues)
				{
					if (baseStarDamageTypeValue.Value != 0f)
					{
						if (item2.BaseStarConditions != null && item2.BaseStarConditions.Any())
						{
							DamageTypeValueModifier damageTypeValueModifier = new DamageTypeValueModifier
							{
								Source = new ItemModifierSource
								{
									WeaponModifierSourceType = Enums.ItemModifierSourceType.ConnectedWeapon,
									ConnectedWeapon = item2
								},
								DamageType = baseStarDamageTypeValue.Key,
								CalculatedBonus = baseStarDamageTypeValue.Value
							};
							ItemModifierFilter itemModifierFilter = new ItemModifierFilter
							{
								Conditions = GetConditionsFromSOs(item2.BaseStarConditions),
								HardTargetWeaponInstance = item
							};
							damageTypeValueModifier.ItemModifierFilter = itemModifierFilter;
							list.Add(damageTypeValueModifier);
						}
						else
						{
							DamageTypeValueModifier damageTypeValueModifier2 = new DamageTypeValueModifier
							{
								Source = new ItemModifierSource
								{
									WeaponModifierSourceType = Enums.ItemModifierSourceType.ConnectedWeapon,
									ConnectedWeapon = item2
								},
								DamageType = baseStarDamageTypeValue.Key,
								CalculatedBonus = baseStarDamageTypeValue.Value
							};
							ItemModifierFilter itemModifierFilter2 = new ItemModifierFilter
							{
								HardTargetWeaponInstance = item
							};
							damageTypeValueModifier2.ItemModifierFilter = itemModifierFilter2;
							list.Add(damageTypeValueModifier2);
						}
					}
				}
			}
		}
		return list;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromBuffStats(List<WeaponInstance> weaponsInBackpack, Dictionary<Enums.ItemStatType, float> buffStats)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		if (buffStats.ContainsKey(Enums.ItemStatType.DamagePercentage))
		{
			DamageTypeValueModifier damageTypeValueModifier = new DamageTypeValueModifier
			{
				Source = new ItemModifierSource
				{
					WeaponModifierSourceType = Enums.ItemModifierSourceType.Buff,
					BuffSO = null
				},
				DamageType = Enums.DamageType.None.AllFlags(),
				CalculatedBonus = buffStats[Enums.ItemStatType.DamagePercentage]
			};
			ItemModifierFilter itemModifierFilter = new ItemModifierFilter
			{
				Conditions = new List<Condition>()
			};
			damageTypeValueModifier.ItemModifierFilter = itemModifierFilter;
			list.Add(damageTypeValueModifier);
		}
		return list;
	}

	private static List<DamageTypeValueModifier> LoadDamageTypeValueModifiersFromBags(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags)
	{
		List<DamageTypeValueModifier> list = new List<DamageTypeValueModifier>();
		foreach (WeaponInstance item2 in weaponsInBackpack)
		{
			if (!weaponsAndContainingBags.ContainsKey(item2))
			{
				continue;
			}
			foreach (BagInstance item3 in weaponsAndContainingBags[item2])
			{
				BagSO bagSO = item3.BagSO;
				foreach (DamageTypeValueModifier item4 in new List<DamageTypeValueModifier>())
				{
					_ = item4;
					foreach (KeyValuePair<Enums.DamageType, float> damageTypeValue in bagSO.Stats.DamageTypeValues)
					{
						if (damageTypeValue.Value != 0f)
						{
							if (bagSO.Stats.Conditions != null)
							{
								DamageTypeValueModifier damageTypeValueModifier = new DamageTypeValueModifier
								{
									Source = new ItemModifierSource
									{
										WeaponModifierSourceType = Enums.ItemModifierSourceType.Bag,
										Bag = bagSO
									},
									DamageType = damageTypeValue.Key,
									CalculatedBonus = damageTypeValue.Value
								};
								ItemModifierFilter itemModifierFilter = new ItemModifierFilter
								{
									Conditions = GetConditionsFromSOs(bagSO.Stats.Conditions)
								};
								damageTypeValueModifier.ItemModifierFilter = itemModifierFilter;
								list.Add(damageTypeValueModifier);
							}
							else
							{
								DamageTypeValueModifier item = new DamageTypeValueModifier
								{
									Source = new ItemModifierSource
									{
										WeaponModifierSourceType = Enums.ItemModifierSourceType.Bag,
										Bag = bagSO
									},
									DamageType = damageTypeValue.Key,
									CalculatedBonus = damageTypeValue.Value
								};
								list.Add(item);
							}
						}
					}
				}
			}
		}
		return list;
	}

	private static void SetPlayerDamageValuesFromModifiers(List<DamageTypeValueModifier> damageTypeValueModifiers, Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> playerDamageValues)
	{
		foreach (Enums.DamageType damageType in Enum.GetValues(typeof(Enums.DamageType)))
		{
			foreach (DamageTypeValueModifier item in damageTypeValueModifiers.Where((DamageTypeValueModifier x) => x.DamageType == damageType))
			{
				if ((item.Source.WeaponModifierSourceType != Enums.ItemModifierSourceType.ConnectedItem || item.ItemModifierFilter == null || item.ItemModifierFilter.HardTargetWeaponInstance == null) && (item.Source.WeaponModifierSourceType != Enums.ItemModifierSourceType.ConnectedWeapon || item.ItemModifierFilter == null || item.ItemModifierFilter.HardTargetWeaponInstance == null) && (item.Source.WeaponModifierSourceType != Enums.ItemModifierSourceType.Bag || item.ItemModifierFilter == null || item.ItemModifierFilter.HardTargetWeaponInstance == null) && (item.Source.WeaponModifierSourceType != Enums.ItemModifierSourceType.ConditionalStatsItem || item.ItemModifierFilter == null || item.ItemModifierFilter.HardTargetWeaponInstance == null))
				{
					playerDamageValues[item.DamageType].Add(item);
				}
			}
		}
	}

	internal static void ApplyDamageModifiersToWeapons(List<WeaponInstance> weapons, List<DamageTypeValueModifier> damageTypeValueModifiers)
	{
		foreach (WeaponInstance weapon in weapons)
		{
			List<DamageModifier> list = new List<DamageModifier>();
			foreach (DamageTypeValueModifier damageTypeValueModifier in damageTypeValueModifiers)
			{
				if (ItemFitsFilter(weapon, damageTypeValueModifier.ItemModifierFilter) && WeaponFitsDamageType(weapon, damageTypeValueModifier))
				{
					DamageModifier item = new DamageModifier
					{
						CalculatedBonus = damageTypeValueModifier.CalculatedBonus,
						Source = damageTypeValueModifier.Source
					};
					list.Add(item);
				}
			}
			float damageMultiplier = weapon.GetCalculatedStat(Enums.WeaponStatType.DamagePercentage);
			list.ForEach(delegate(DamageModifier dm)
			{
				dm.CalculatedBonus *= damageMultiplier;
			});
			weapon.AddDamageModifiers(list);
		}
	}

	private static bool WeaponFitsDamageType(WeaponInstance weaponInstance, DamageTypeValueModifier damageTypeValueModifier)
	{
		return (weaponInstance.DamageInstance.CalculatedDamageType & damageTypeValueModifier.DamageType) == damageTypeValueModifier.DamageType;
	}

	private static List<WeaponStatModifier> CalculateStatModifiersForWeapons(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> basePlayerStats, List<WeaponInstance> weaponsInBackpack, List<WeaponInstance> weaponsInShop, List<WeaponInstance> weaponsInStorage, List<ItemInstance> itemsInBackpack, List<RelicSO> relics, List<TalentSO> talents, Dictionary<WeaponInstance, List<ItemInstance>> weaponsAndItemStarSources, Dictionary<WeaponInstance, List<WeaponInstance>> weaponsAndWeaponStarSources, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags, Dictionary<Enums.ItemStatType, float> buffStats)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		list.AddRange(LoadWeaponStatModifiersFromPlayerStats(basePlayerStats));
		list.AddRange(LoadWeaponStatModifiersFromGlobalItems(itemsInBackpack));
		list.AddRange(LoadWeaponStatModifiersFromConditionalStatsInItems(itemsInBackpack));
		list.AddRange(LoadWeaponStatModifiersFromRelics(relics));
		list.AddRange(LoadWeaponStatModifiersFromStarredItems(weaponsInBackpack, weaponsAndItemStarSources));
		list.AddRange(LoadWeaponStatModifiersFromStarredWeapons(weaponsInBackpack, weaponsAndWeaponStarSources));
		list.AddRange(LoadWeaponStatModifiersFromBags(weaponsInBackpack, weaponsAndContainingBags));
		list.AddRange(LoadWeaponStatModifiersFromBuffStats(buffStats));
		list.AddRange(LoadWeaponStatModifiersFromTalents(talents));
		list.AddRange(LoadWeaponFormulaStatModifiers(weaponsInBackpack));
		list.AddRange(LoadWeaponFormulaStatModifiers(weaponsInShop));
		list.AddRange(LoadWeaponFormulaStatModifiers(weaponsInStorage));
		return list;
	}

	private static IEnumerable<WeaponStatModifier> LoadWeaponFormulaStatModifiers(List<WeaponInstance> weapons)
	{
		return CalculateWeaponFormulaStats(weapons);
	}

	private static IEnumerable<WeaponStatModifier> LoadWeaponStatModifiersFromConditionalStatsInItems(List<ItemInstance> itemsInBackpack)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (ItemInstance item in itemsInBackpack)
		{
			foreach (KeyValuePair<Enums.ItemStatType, float> conditionalStatValue in item.ConditionalStatValues)
			{
				WeaponStatModifier weaponStatModifier = new WeaponStatModifier();
				if (TryGetWeaponStatTypeFromItemStatType(conditionalStatValue.Key, out var weaponStatType))
				{
					weaponStatModifier.WeaponStatType = weaponStatType;
					weaponStatModifier.CalculatedBonus = conditionalStatValue.Value;
					ItemModifierFilter itemModifierFilter = new ItemModifierFilter
					{
						Conditions = GetConditionsFromSOs(item.ConditionalStatConditions)
					};
					weaponStatModifier.ItemModifierFilter = itemModifierFilter;
					ItemModifierSource itemModifierSource = new ItemModifierSource();
					itemModifierSource.WeaponModifierSourceType = Enums.ItemModifierSourceType.ConditionalStatsItem;
					itemModifierSource.ConditionalStatsItem = item;
					weaponStatModifier.Source = itemModifierSource;
					list.Add(weaponStatModifier);
				}
			}
			if (item.ConditionalFormulaStatValues == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> conditionalFormulaStatValue in item.ConditionalFormulaStatValues)
			{
				WeaponStatModifier weaponStatModifier2 = new WeaponStatModifier();
				if (TryGetWeaponStatTypeFromItemStatType(conditionalFormulaStatValue.Key, out var weaponStatType2))
				{
					weaponStatModifier2.WeaponStatType = weaponStatType2;
					weaponStatModifier2.CalculatedBonus = conditionalFormulaStatValue.Value.GetFormulaResult();
					ItemModifierFilter itemModifierFilter2 = new ItemModifierFilter
					{
						Conditions = GetConditionsFromSOs(item.ConditionalStatConditions)
					};
					weaponStatModifier2.ItemModifierFilter = itemModifierFilter2;
					ItemModifierSource itemModifierSource2 = new ItemModifierSource();
					itemModifierSource2.WeaponModifierSourceType = Enums.ItemModifierSourceType.ConditionalFormula;
					itemModifierSource2.ConditionalStatsItem = item;
					weaponStatModifier2.Source = itemModifierSource2;
					list.Add(weaponStatModifier2);
				}
			}
		}
		return list;
	}

	private static IEnumerable<WeaponStatModifier> LoadWeaponStatModifiersFromTalents(List<TalentSO> talents)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (TalentSO talent in talents)
		{
			if (talent?.Stats == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, float> statValue in talent.Stats.StatValues)
			{
				WeaponStatModifier weaponStatModifier = new WeaponStatModifier();
				if (TryGetWeaponStatTypeFromItemStatType(statValue.Key, out var weaponStatType))
				{
					weaponStatModifier.WeaponStatType = weaponStatType;
					weaponStatModifier.CalculatedBonus = statValue.Value;
					if (talent.Stats.Conditions != null && talent.Stats.Conditions.Count() > 0)
					{
						ItemModifierFilter itemModifierFilter = new ItemModifierFilter
						{
							Conditions = GetConditionsFromSOs(talent.Stats.Conditions)
						};
						weaponStatModifier.ItemModifierFilter = itemModifierFilter;
					}
					ItemModifierSource itemModifierSource = new ItemModifierSource();
					itemModifierSource.WeaponModifierSourceType = Enums.ItemModifierSourceType.Talent;
					itemModifierSource.Talent = talent;
					weaponStatModifier.Source = itemModifierSource;
					list.Add(weaponStatModifier);
				}
			}
		}
		foreach (TalentSO talent2 in talents)
		{
			if (talent2?.Stats?.FormulaStats == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> formulaStat in talent2.Stats.FormulaStats)
			{
				WeaponStatModifier weaponStatModifier2 = new WeaponStatModifier();
				if (TryGetWeaponStatTypeFromItemStatType(formulaStat.Key, out var weaponStatType2))
				{
					weaponStatModifier2.WeaponStatType = weaponStatType2;
					weaponStatModifier2.CalculatedBonus = formulaStat.Value.GetFormulaResult();
					if (talent2.Stats.Conditions != null && talent2.Stats.Conditions.Count() > 0)
					{
						ItemModifierFilter itemModifierFilter2 = new ItemModifierFilter
						{
							Conditions = GetConditionsFromSOs(talent2.Stats.Conditions)
						};
						weaponStatModifier2.ItemModifierFilter = itemModifierFilter2;
					}
					ItemModifierSource itemModifierSource2 = new ItemModifierSource();
					itemModifierSource2.WeaponModifierSourceType = Enums.ItemModifierSourceType.Talent;
					itemModifierSource2.Talent = talent2;
					weaponStatModifier2.Source = itemModifierSource2;
					list.Add(weaponStatModifier2);
				}
			}
		}
		return list;
	}

	private static IEnumerable<WeaponStatModifier> LoadWeaponStatModifiersFromBuffStats(Dictionary<Enums.ItemStatType, float> buffStats)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (KeyValuePair<Enums.ItemStatType, float> buffStat in buffStats)
		{
			if (TryGetWeaponStatTypeFromItemStatType(buffStat.Key, out var weaponStatType))
			{
				WeaponStatModifier weaponStatModifier = new WeaponStatModifier();
				weaponStatModifier.WeaponStatType = weaponStatType;
				weaponStatModifier.CalculatedBonus = buffStat.Value;
				list.Add(weaponStatModifier);
			}
		}
		return list;
	}

	private static IEnumerable<WeaponStatModifier> LoadWeaponStatModifiersFromPlayerStats(Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (KeyValuePair<Enums.ItemStatType, List<ItemStatModifier>> playerStat in playerStats)
		{
			if (!TryGetWeaponStatTypeFromItemStatType(playerStat.Key, out var weaponStatType))
			{
				continue;
			}
			foreach (ItemStatModifier item in playerStat.Value)
			{
				WeaponStatModifier weaponStatModifier = new WeaponStatModifier();
				weaponStatModifier.WeaponStatType = weaponStatType;
				weaponStatModifier.CalculatedBonus = item.CalculatedBonus;
				list.Add(weaponStatModifier);
			}
		}
		return list;
	}

	private static List<WeaponStatModifier> LoadWeaponStatModifiersFromRelics(List<RelicSO> relics)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (RelicSO relic in relics)
		{
			if (relic.GlobalStats == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, float> statValue in relic.GlobalStats.StatValues)
			{
				if (TryGetWeaponStatTypeFromItemStatType(statValue.Key, out var weaponStatType))
				{
					List<WeaponStatModifier> weaponStatModifiers = GetWeaponStatModifiers(null, weaponStatType, statValue.Value, Enums.ItemModifierSourceType.Relic, relic.Conditions, null, null, null, relic);
					list.AddRange(weaponStatModifiers);
				}
			}
		}
		return list;
	}

	private static List<WeaponStatModifier> LoadWeaponStatModifiersFromStarredItems(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstanceSets)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!linkedWeaponsAndItemInstanceSets.ContainsKey(item))
			{
				continue;
			}
			foreach (ItemInstance item2 in linkedWeaponsAndItemInstanceSets[item])
			{
				foreach (KeyValuePair<Enums.ItemStatType, float> starStatValue in item2.StarStatValues)
				{
					if (TryGetWeaponStatTypeFromItemStatType(starStatValue.Key, out var weaponStatType))
					{
						List<WeaponStatModifier> weaponStatModifiers = GetWeaponStatModifiers(item, weaponStatType, starStatValue.Value, Enums.ItemModifierSourceType.ConnectedItem, item2.StarWeaponConditions, null, item2, null, null, null, null, hardLinkFilterToWeapon: true);
						list.AddRange(weaponStatModifiers);
					}
				}
				if (item2.StarredFormulaStatValues == null)
				{
					continue;
				}
				foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> starredFormulaStatValue in item2.StarredFormulaStatValues)
				{
					if (TryGetWeaponStatTypeFromItemStatType(starredFormulaStatValue.Key, out var weaponStatType2))
					{
						List<WeaponStatModifier> weaponStatModifiers2 = GetWeaponStatModifiers(item, weaponStatType2, starredFormulaStatValue.Value.GetFormulaResult(), Enums.ItemModifierSourceType.ConnectedItem, item2.StarWeaponConditions, null, item2, null, null, null, null, hardLinkFilterToWeapon: true);
						list.AddRange(weaponStatModifiers2);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponStatModifier> LoadWeaponStatModifiersFromStarredWeapons(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<WeaponInstance>> weaponsAndWeaponStarSources)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!weaponsAndWeaponStarSources.ContainsKey(item))
			{
				continue;
			}
			foreach (WeaponInstance item2 in weaponsAndWeaponStarSources[item])
			{
				foreach (KeyValuePair<Enums.ItemStatType, float> baseStarStatValue in item2.BaseStarStatValues)
				{
					if (TryGetWeaponStatTypeFromItemStatType(baseStarStatValue.Key, out var weaponStatType))
					{
						List<WeaponStatModifier> weaponStatModifiers = GetWeaponStatModifiers(item, weaponStatType, baseStarStatValue.Value, Enums.ItemModifierSourceType.ConnectedWeapon, item2.BaseStarConditions, item2, null, null, null, null, null, hardLinkFilterToWeapon: true);
						list.AddRange(weaponStatModifiers);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponStatModifier> LoadWeaponStatModifiersFromGlobalItems(List<ItemInstance> itemsInBackpack)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (ItemInstance item in itemsInBackpack)
		{
			foreach (KeyValuePair<Enums.ItemStatType, float> globalStatValue in item.GlobalStatValues)
			{
				if (TryGetWeaponStatTypeFromItemStatType(globalStatValue.Key, out var weaponStatType))
				{
					List<WeaponStatModifier> weaponStatModifiers = GetWeaponStatModifiers(null, weaponStatType, globalStatValue.Value, Enums.ItemModifierSourceType.GlobalItem, item.ConditionalStatConditions, null, null, item);
					list.AddRange(weaponStatModifiers);
				}
			}
			if (item.GlobalFormulaStatValues == null)
			{
				continue;
			}
			foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> globalFormulaStatValue in item.GlobalFormulaStatValues)
			{
				if (TryGetWeaponStatTypeFromItemStatType(globalFormulaStatValue.Key, out var weaponStatType2))
				{
					List<WeaponStatModifier> weaponStatModifiers2 = GetWeaponStatModifiers(null, weaponStatType2, globalFormulaStatValue.Value.GetFormulaResult(), Enums.ItemModifierSourceType.GlobalItem, item.ConditionalStatConditions, null, null, item);
					list.AddRange(weaponStatModifiers2);
				}
			}
		}
		return list;
	}

	private static List<WeaponStatModifier> LoadWeaponStatModifiersFromBags(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!weaponsAndContainingBags.ContainsKey(item))
			{
				continue;
			}
			foreach (BagInstance item2 in weaponsAndContainingBags[item])
			{
				BagSO bagSO = item2.BagSO;
				if (bagSO.Stats == null)
				{
					continue;
				}
				foreach (KeyValuePair<Enums.ItemStatType, float> statValue in bagSO.Stats.StatValues)
				{
					if (TryGetWeaponStatTypeFromItemStatType(statValue.Key, out var weaponStatType))
					{
						List<WeaponStatModifier> weaponStatModifiers = GetWeaponStatModifiers(null, weaponStatType, statValue.Value, Enums.ItemModifierSourceType.Bag, bagSO.Stats.Conditions, null, null, null, null, null, bagSO, hardLinkFilterToWeapon: true);
						list.AddRange(weaponStatModifiers);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponStatModifier> GetWeaponStatModifiers(WeaponInstance weaponInBackpack, Enums.WeaponStatType weaponStatType, float calulatedBonus, Enums.ItemModifierSourceType itemModifierSourceType, ConditionSO[] conditionSOs, WeaponInstance connectedWeapon = null, ItemInstance connectedItem = null, ItemInstance globalItem = null, RelicSO relicSO = null, TalentSO talentSO = null, BagSO bagSO = null, bool hardLinkFilterToWeapon = false, ItemInstance weaponFilterItem = null)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		if (!CanCreateValidWeaponModifier(itemModifierSourceType, connectedWeapon, connectedItem, globalItem, relicSO, talentSO, bagSO, weaponFilterItem))
		{
			Debug.LogError($"A valid WeaponStatModifier cannot be created for weapon with guid {weaponInBackpack.Guid} and ItemModifierSourceType {itemModifierSourceType}");
			return list;
		}
		if (calulatedBonus == 0f)
		{
			return list;
		}
		WeaponStatModifier weaponStatModifier = new WeaponStatModifier
		{
			Source = new ItemModifierSource
			{
				WeaponModifierSourceType = itemModifierSourceType,
				ConnectedWeapon = connectedWeapon,
				ConnectedItem = connectedItem,
				GlobalItem = globalItem,
				Relic = relicSO,
				Talent = talentSO,
				Bag = bagSO,
				ConditionalStatsItem = weaponFilterItem
			},
			WeaponStatType = weaponStatType,
			CalculatedBonus = calulatedBonus
		};
		if (hardLinkFilterToWeapon)
		{
			AddHardTargetWeaponFilter(weaponStatModifier, weaponInBackpack);
		}
		if (!conditionSOs.Any())
		{
			list.Add(weaponStatModifier);
			return list;
		}
		for (int i = 0; i < conditionSOs.Length; i++)
		{
			_ = conditionSOs[i];
			WeaponStatModifier weaponStatModifier2 = WeaponStatModifier.Clone(weaponStatModifier);
			ItemModifierFilter itemModifierFilter = new ItemModifierFilter
			{
				Conditions = GetConditionsFromSOs(conditionSOs),
				HardTargetWeaponInstance = weaponInBackpack
			};
			weaponStatModifier2.ItemModifierFilter = itemModifierFilter;
			list.Add(weaponStatModifier2);
		}
		return list;
	}

	private static void AddHardTargetWeaponFilter(WeaponStatModifier weaponStatModifier, WeaponInstance weaponInstance)
	{
		ItemModifierFilter itemModifierFilter = new ItemModifierFilter
		{
			HardTargetWeaponInstance = weaponInstance
		};
		weaponStatModifier.ItemModifierFilter = itemModifierFilter;
	}

	private static bool CanCreateValidWeaponModifier(Enums.ItemModifierSourceType itemModifierSourceType, WeaponInstance connectedWeapon, ItemInstance connectedItem, ItemInstance globalItem, RelicSO relicSO, TalentSO talentSO, BagSO bagSO, ItemInstance weaponFilterItem)
	{
		return itemModifierSourceType switch
		{
			Enums.ItemModifierSourceType.ConnectedWeapon => connectedWeapon != null, 
			Enums.ItemModifierSourceType.ConnectedItem => connectedItem != null, 
			Enums.ItemModifierSourceType.GlobalItem => globalItem != null, 
			Enums.ItemModifierSourceType.Relic => relicSO != null, 
			Enums.ItemModifierSourceType.Talent => talentSO != null, 
			Enums.ItemModifierSourceType.Bag => bagSO != null, 
			Enums.ItemModifierSourceType.ConditionalStatsItem => weaponFilterItem != null, 
			_ => false, 
		};
	}

	private static void ResetWeaponStats(List<WeaponInstance> weaponsInBackpack)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			item.ResetStats();
		}
	}

	private static void ApplyStatModifiersToWeapons(List<WeaponInstance> weaponsInBackpack, List<WeaponStatModifier> baseWeaponStatModifiers)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (WeaponStatModifier baseWeaponStatModifier in baseWeaponStatModifiers)
			{
				if (ItemFitsFilter(item, baseWeaponStatModifier.ItemModifierFilter))
				{
					item.AddModifier(baseWeaponStatModifier);
				}
			}
		}
	}

	private static void ApplyWeaponAttackEffectsToWeapons(List<WeaponInstance> weaponsInBackpack, List<WeaponAttackEffectModifier> weaponAttackEffectModifiers)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (WeaponAttackEffectModifier weaponAttackEffectModifier in weaponAttackEffectModifiers)
			{
				if (ItemFitsFilter(item, weaponAttackEffectModifier.ItemModifierFilter))
				{
					item.AddWeaponAttackEffect(weaponAttackEffectModifier);
				}
			}
		}
	}

	private static void AddWeaponDebuffsToWeapons(List<WeaponInstance> weaponsInBackpack, List<WeaponDebuffModifier> weaponDebuffModifiers)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (WeaponDebuffModifier weaponDebuffModifier in weaponDebuffModifiers)
			{
				if (ItemFitsFilter(item, weaponDebuffModifier.ItemModifierFilter))
				{
					item.AddWeaponDebuff(weaponDebuffModifier);
				}
			}
		}
	}

	private static void SetDamageOverrides(List<ItemInstance> itemsInBackpack, List<WeaponInstance> weaponsInBackpack, List<RelicSO> activeRelics, List<TalentSO> activeTalents, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstance)
	{
		SetDamageTypeOverridesFromGlobalItems(weaponsInBackpack, itemsInBackpack);
		SetDamageTypeOverridesFromTalents(weaponsInBackpack, activeTalents);
		SetDamageTypeOverridesFromRelics(weaponsInBackpack, activeRelics);
		SetDamageTypeOverridesFromBags(weaponsInBackpack);
		SetDamageTypeOverridesFromStarredItems(weaponsInBackpack, linkedWeaponsAndItemInstance);
	}

	private static void SetDamageTypeOverridesFromGlobalItems(List<WeaponInstance> weaponsInBackpack, List<ItemInstance> itemsInBackpack)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (ItemInstance item2 in itemsInBackpack)
			{
				if (item2.ConditionalWeaponDamageTypeValueOverrides == null)
				{
					continue;
				}
				if (item2.ConditionalStatConditions != null && item2.ConditionalStatConditions.Any())
				{
					ItemModifierFilter filter = new ItemModifierFilter
					{
						Conditions = GetConditionsFromSOs(item2.ConditionalStatConditions)
					};
					if (ItemFitsFilter(item, filter))
					{
						WeaponDamageTypeValueOverride[] conditionalWeaponDamageTypeValueOverrides = item2.ConditionalWeaponDamageTypeValueOverrides;
						foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in conditionalWeaponDamageTypeValueOverrides)
						{
							item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride.TargetWeaponDamageType;
						}
					}
				}
				else
				{
					WeaponDamageTypeValueOverride[] conditionalWeaponDamageTypeValueOverrides = item2.ConditionalWeaponDamageTypeValueOverrides;
					foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride2 in conditionalWeaponDamageTypeValueOverrides)
					{
						item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride2.TargetWeaponDamageType;
					}
				}
			}
		}
	}

	private static void SetDamageTypeOverridesFromStarredItems(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstanceSets)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!linkedWeaponsAndItemInstanceSets.ContainsKey(item))
			{
				continue;
			}
			foreach (ItemInstance item2 in linkedWeaponsAndItemInstanceSets[item])
			{
				WeaponDamageTypeValueOverride[] starWeaponDamageTypeValueOverrides = item2.StarWeaponDamageTypeValueOverrides;
				foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in starWeaponDamageTypeValueOverrides)
				{
					item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride.TargetWeaponDamageType;
				}
			}
		}
	}

	private static void SetDamageTypeOverridesFromBags(List<WeaponInstance> weaponsInBackpack)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (BagSO item2 in new List<BagSO>())
			{
				new List<WeaponStatModifier>();
				ItemModifierFilter filter = new ItemModifierFilter
				{
					Conditions = GetConditionsFromSOs(item2.Stats.Conditions)
				};
				if (ItemFitsFilter(item, filter))
				{
					WeaponDamageTypeValueOverride[] weaponDamageTypeValueOverrides = item2.Stats.WeaponDamageTypeValueOverrides;
					foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in weaponDamageTypeValueOverrides)
					{
						item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride.TargetWeaponDamageType;
					}
				}
			}
		}
	}

	private static void SetDamageTypeOverridesFromRelics(List<WeaponInstance> weaponsInBackpack, List<RelicSO> activeRelics)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (RelicSO activeRelic in activeRelics)
			{
				if (activeRelic.WeaponDamageTypeCalculationOverrides == null)
				{
					continue;
				}
				ItemModifierFilter filter = new ItemModifierFilter
				{
					Conditions = GetConditionsFromSOs(activeRelic.Conditions)
				};
				if (ItemFitsFilter(item, filter))
				{
					WeaponDamageTypeValueOverride[] weaponDamageTypeCalculationOverrides = activeRelic.WeaponDamageTypeCalculationOverrides;
					foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in weaponDamageTypeCalculationOverrides)
					{
						item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride.TargetWeaponDamageType;
					}
				}
			}
		}
	}

	private static void SetDamageTypeOverridesFromTalents(List<WeaponInstance> weaponsInBackpack, List<TalentSO> activeTalents)
	{
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			foreach (TalentSO activeTalent in activeTalents)
			{
				if (activeTalent.Stats == null)
				{
					continue;
				}
				if (activeTalent?.Stats?.Conditions == null)
				{
					WeaponDamageTypeValueOverride[] weaponDamageTypeValueOverrides = activeTalent.Stats.WeaponDamageTypeValueOverrides;
					foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in weaponDamageTypeValueOverrides)
					{
						item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride.TargetWeaponDamageType;
					}
					continue;
				}
				ItemModifierFilter filter = new ItemModifierFilter
				{
					Conditions = GetConditionsFromSOs(activeTalent.Stats.Conditions)
				};
				if (ItemFitsFilter(item, filter))
				{
					WeaponDamageTypeValueOverride[] weaponDamageTypeValueOverrides = activeTalent.Stats.WeaponDamageTypeValueOverrides;
					foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride2 in weaponDamageTypeValueOverrides)
					{
						item.DamageInstance.CalculatedDamageType = weaponDamageTypeValueOverride2.TargetWeaponDamageType;
					}
				}
			}
		}
	}

	private static void SetModifiersFromOverrides(List<WeaponInstance> weapons, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		foreach (WeaponInstance weapon in weapons)
		{
			foreach (WeaponStatModifier item in LoadStatModifiersFromWeaponCalculationOverrides(weapon, playerStats))
			{
				weapon.SetModifier(item);
			}
		}
	}

	private static List<WeaponStatModifier> LoadStatModifiersFromWeaponCalculationOverrides(WeaponInstance weaponInstance, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		List<WeaponStatModifier> list = new List<WeaponStatModifier>();
		WeaponStatTypeCalculationOverride[] baseWeaponStatTypeCalculationOverrides = weaponInstance.BaseWeaponStatTypeCalculationOverrides;
		foreach (WeaponStatTypeCalculationOverride weaponStatTypeCalculationOverride in baseWeaponStatTypeCalculationOverrides)
		{
			WeaponStatModifier weaponStatModifier = new WeaponStatModifier();
			weaponStatModifier.WeaponStatType = weaponStatTypeCalculationOverride.TargetWeaponStatType;
			weaponInstance.GetCalculatedStat(weaponStatModifier.WeaponStatType);
			float num = 0f;
			WeaponStatTypeCalculationOverrideSource[] weaponStatTypeCalculationOverrideSources = weaponStatTypeCalculationOverride.WeaponStatTypeCalculationOverrideSources;
			foreach (WeaponStatTypeCalculationOverrideSource weaponStatTypeCalculationOverrideSource in weaponStatTypeCalculationOverrideSources)
			{
				float num2 = playerStats[weaponStatTypeCalculationOverrideSource.WeaponStatType].Sum((ItemStatModifier x) => x.CalculatedBonus);
				num += num2 * weaponStatTypeCalculationOverrideSource.Value;
			}
			switch (weaponStatTypeCalculationOverride.OverrideOrAdd)
			{
			case Enums.OverrideOrAdd.Override:
				weaponStatModifier.CalculatedBonus = num;
				break;
			case Enums.OverrideOrAdd.Add:
				if (!weaponInstance.BaseStatValues.ContainsKey(weaponStatTypeCalculationOverride.TargetWeaponStatType))
				{
					weaponInstance.BaseStatValues.Add(weaponStatTypeCalculationOverride.TargetWeaponStatType, 0f);
				}
				weaponStatModifier.CalculatedBonus = num + weaponInstance.BaseStatValues[weaponStatTypeCalculationOverride.TargetWeaponStatType];
				break;
			}
			weaponStatModifier.Source = new ItemModifierSource
			{
				WeaponModifierSourceType = Enums.ItemModifierSourceType.ModifierOverride
			};
			list.Add(weaponStatModifier);
		}
		return list;
	}

	private static void SetDamageFromOverrides(List<WeaponInstance> weapons, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> playerStats)
	{
		foreach (WeaponInstance weapon in weapons)
		{
			if (!weapon.BaseWeaponDamageCalculationOverrides.Any())
			{
				continue;
			}
			float num = 0f;
			float num2 = 0f;
			WeaponDamageCalculationOverride[] baseWeaponDamageCalculationOverrides = weapon.BaseWeaponDamageCalculationOverrides;
			for (int i = 0; i < baseWeaponDamageCalculationOverrides.Length; i++)
			{
				WeaponDamageCalculationOverrideSource[] weaponDamageCalculationOverrideSources = baseWeaponDamageCalculationOverrides[i].WeaponDamageCalculationOverrideSources;
				foreach (WeaponDamageCalculationOverrideSource weaponDamageCalculationOverrideSource in weaponDamageCalculationOverrideSources)
				{
					float num3 = playerStats[weaponDamageCalculationOverrideSource.WeaponStatType].Sum((ItemStatModifier x) => x.CalculatedBonus);
					num += num3 * weaponDamageCalculationOverrideSource.MinValue;
					num2 += num3 * weaponDamageCalculationOverrideSource.MaxValue;
				}
			}
			weapon.SetMinMaxDamage(num, num2);
		}
	}

	private static List<WeaponAttackEffectModifier> CalculateWeaponAttackEffectModifiersForWeapons(List<WeaponInstance> weaponsInBackpack, List<ItemInstance> itemsInBackpack, List<RelicSO> relics, List<TalentSO> talents, Dictionary<WeaponInstance, List<ItemInstance>> weaponsAndItemStarSources, Dictionary<WeaponInstance, List<WeaponInstance>> weaponsAndWeaponStarSources, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		list.AddRange(LoadWeaponAttackEffectModifiersFromGlobalItems(itemsInBackpack));
		list.AddRange(LoadWeaponAttackEffectModifiersFromRelics(relics));
		list.AddRange(LoadWeaponAttackEffectModifiersFromTalents(talents));
		list.AddRange(LoadWeaponAttackEffectModifiersFromStarredItems(weaponsInBackpack, weaponsAndItemStarSources));
		list.AddRange(LoadWeaponAttackEffectModifiersFromStarredWeapons(weaponsInBackpack, weaponsAndWeaponStarSources));
		list.AddRange(LoadWeaponAttackEffectModifiersFromBags(weaponsInBackpack, weaponsAndContainingBags));
		return list;
	}

	private static List<WeaponAttackEffectModifier> LoadWeaponAttackEffectModifiersFromGlobalItems(List<ItemInstance> itemsInBackpack)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		foreach (ItemInstance item in itemsInBackpack)
		{
			if (item.GlobalAttackEffects != null)
			{
				WeaponAttackEffect[] globalAttackEffects = item.GlobalAttackEffects;
				foreach (WeaponAttackEffect weaponAttackEffect in globalAttackEffects)
				{
					List<WeaponAttackEffectModifier> weaponAttackEffectModifier = GetWeaponAttackEffectModifier(null, weaponAttackEffect, Enums.ItemModifierSourceType.GlobalItem, new ConditionSO[0], null, null, item);
					list.AddRange(weaponAttackEffectModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponAttackEffectModifier> LoadWeaponAttackEffectModifiersFromBags(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!weaponsAndContainingBags.ContainsKey(item))
			{
				continue;
			}
			foreach (BagInstance item2 in weaponsAndContainingBags[item])
			{
				BagSO bagSO = item2.BagSO;
				if (!(bagSO.Stats == null))
				{
					WeaponAttackEffect[] weaponAttackEffects = bagSO.Stats.WeaponAttackEffects;
					foreach (WeaponAttackEffect weaponAttackEffect in weaponAttackEffects)
					{
						List<WeaponAttackEffectModifier> weaponAttackEffectModifier = GetWeaponAttackEffectModifier(null, weaponAttackEffect, Enums.ItemModifierSourceType.Bag, bagSO.Stats.Conditions, null, null, null, null, null, bagSO, hardLinkFilterToWeapon: true);
						list.AddRange(weaponAttackEffectModifier);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponAttackEffectModifier> LoadWeaponAttackEffectModifiersFromRelics(List<RelicSO> relics)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		foreach (RelicSO relic in relics)
		{
			if (!(relic.GlobalStats == null) && relic.GlobalStats.WeaponAttackEffects != null)
			{
				WeaponAttackEffect[] weaponAttackEffects = relic.GlobalStats.WeaponAttackEffects;
				foreach (WeaponAttackEffect weaponAttackEffect in weaponAttackEffects)
				{
					List<WeaponAttackEffectModifier> weaponAttackEffectModifier = GetWeaponAttackEffectModifier(null, weaponAttackEffect, Enums.ItemModifierSourceType.Relic, relic.Conditions, null, null, null, relic);
					list.AddRange(weaponAttackEffectModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponAttackEffectModifier> LoadWeaponAttackEffectModifiersFromTalents(List<TalentSO> talents)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		foreach (TalentSO talent in talents)
		{
			if (!(talent.Stats == null))
			{
				WeaponAttackEffect[] weaponAttackEffects = talent.Stats.WeaponAttackEffects;
				foreach (WeaponAttackEffect weaponAttackEffect in weaponAttackEffects)
				{
					List<WeaponAttackEffectModifier> weaponAttackEffectModifier = GetWeaponAttackEffectModifier(null, weaponAttackEffect, Enums.ItemModifierSourceType.Talent, talent.Stats.Conditions, null, null, null, null, talent);
					list.AddRange(weaponAttackEffectModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponAttackEffectModifier> LoadWeaponAttackEffectModifiersFromStarredItems(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstanceSets)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!linkedWeaponsAndItemInstanceSets.ContainsKey(item))
			{
				continue;
			}
			foreach (ItemInstance item2 in linkedWeaponsAndItemInstanceSets[item])
			{
				if (item2.StarWeaponAttackEffects != null)
				{
					WeaponAttackEffect[] starWeaponAttackEffects = item2.StarWeaponAttackEffects;
					foreach (WeaponAttackEffect weaponAttackEffect in starWeaponAttackEffects)
					{
						List<WeaponAttackEffectModifier> weaponAttackEffectModifier = GetWeaponAttackEffectModifier(item, weaponAttackEffect, Enums.ItemModifierSourceType.ConnectedItem, item2.StarWeaponConditions, null, item2, null, null, null, null, hardLinkFilterToWeapon: true);
						list.AddRange(weaponAttackEffectModifier);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponAttackEffectModifier> LoadWeaponAttackEffectModifiersFromStarredWeapons(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<WeaponInstance>> linkedWeaponsAndWeaponInstanceSets)
	{
		return new List<WeaponAttackEffectModifier>();
	}

	private static List<WeaponAttackEffectModifier> GetWeaponAttackEffectModifier(WeaponInstance weaponInBackpack, WeaponAttackEffect weaponAttackEffect, Enums.ItemModifierSourceType itemModifierSourceType, ConditionSO[] conditions, WeaponInstance connectedWeapon = null, ItemInstance connectedItem = null, ItemInstance globalItem = null, RelicSO relicSO = null, TalentSO talentSO = null, BagSO bagSO = null, bool hardLinkFilterToWeapon = false, ItemInstance weaponFilterItem = null)
	{
		List<WeaponAttackEffectModifier> list = new List<WeaponAttackEffectModifier>();
		if (!CanCreateValidWeaponModifier(itemModifierSourceType, connectedWeapon, connectedItem, globalItem, relicSO, talentSO, bagSO, weaponFilterItem))
		{
			Debug.LogError($"A valid WeaponStatModifier cannot be created for weapon with guid {weaponInBackpack.Guid} and ItemModifierSourceType {itemModifierSourceType}");
			return list;
		}
		WeaponAttackEffectModifier weaponAttackEffectModifier = new WeaponAttackEffectModifier
		{
			Source = new ItemModifierSource
			{
				WeaponModifierSourceType = itemModifierSourceType,
				ConnectedWeapon = connectedWeapon,
				ConnectedItem = connectedItem,
				GlobalItem = globalItem,
				Relic = relicSO,
				Talent = talentSO,
				Bag = bagSO,
				ConditionalStatsItem = weaponFilterItem
			},
			WeaponAttackEffect = weaponAttackEffect
		};
		if (hardLinkFilterToWeapon)
		{
			AddHardTargetWeaponEffectFilter(weaponAttackEffectModifier, weaponInBackpack);
		}
		if (!conditions.Any())
		{
			list.Add(weaponAttackEffectModifier);
			return list;
		}
		WeaponAttackEffectModifier weaponAttackEffectModifier2 = WeaponAttackEffectModifier.Clone(weaponAttackEffectModifier);
		ItemModifierFilter itemModifierFilter = new ItemModifierFilter
		{
			HardTargetWeaponInstance = weaponInBackpack,
			Conditions = GetConditionsFromSOs(conditions)
		};
		weaponAttackEffectModifier2.ItemModifierFilter = itemModifierFilter;
		list.Add(weaponAttackEffectModifier2);
		return list;
	}

	private static void AddHardTargetWeaponEffectFilter(WeaponAttackEffectModifier weaponAttackEffectModifier, WeaponInstance weaponInstance)
	{
		ItemModifierFilter itemModifierFilter = new ItemModifierFilter
		{
			HardTargetWeaponInstance = weaponInstance
		};
		weaponAttackEffectModifier.ItemModifierFilter = itemModifierFilter;
	}

	private static List<WeaponDebuffModifier> CalculateWeaponDebuffModifiersForWeapons(List<WeaponInstance> weaponsInBackpack, List<ItemInstance> itemsInBackpack, List<RelicSO> relics, List<TalentSO> talents, Dictionary<WeaponInstance, List<ItemInstance>> weaponsAndItemStarSources, Dictionary<WeaponInstance, List<WeaponInstance>> weaponsAndWeaponStarSources, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		list.AddRange(LoadWeaponDebuffModifiersFromGlobalItems(itemsInBackpack));
		list.AddRange(LoadWeaponDebuffModifiersFromRelics(relics));
		list.AddRange(LoadWeaponDebuffModifiersFromTalents(talents));
		list.AddRange(LoadWeaponDebuffModifiersFromWeaponFilterItems(itemsInBackpack));
		list.AddRange(LoadWeaponDebuffModifiersFromStarredItems(weaponsInBackpack, weaponsAndItemStarSources));
		list.AddRange(LoadWeaponDebuffModifiersFromBags(weaponsInBackpack, weaponsAndContainingBags));
		return list;
	}

	private static List<WeaponDebuffModifier> LoadWeaponDebuffModifiersFromWeaponFilterItems(List<ItemInstance> itemsInBackpack)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		foreach (ItemInstance item in itemsInBackpack)
		{
			if (item.ConditionalDebuffSOs != null)
			{
				DebuffSO[] conditionalDebuffSOs = item.ConditionalDebuffSOs;
				foreach (DebuffSO debuffSO in conditionalDebuffSOs)
				{
					List<WeaponDebuffModifier> weaponDebuffModifier = GetWeaponDebuffModifier(null, debuffSO, Enums.ItemModifierSourceType.ConditionalStatsItem, item.ConditionalStatConditions, null, null, null, null, null, null, hardLinkFilterToWeapon: false, item);
					list.AddRange(weaponDebuffModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponDebuffModifier> LoadWeaponDebuffModifiersFromGlobalItems(List<ItemInstance> itemsInBackpack)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		foreach (ItemInstance item in itemsInBackpack)
		{
			if (item.GlobalDebuffSOs != null)
			{
				DebuffSO[] globalDebuffSOs = item.GlobalDebuffSOs;
				foreach (DebuffSO debuffSO in globalDebuffSOs)
				{
					List<WeaponDebuffModifier> weaponDebuffModifier = GetWeaponDebuffModifier(null, debuffSO, Enums.ItemModifierSourceType.GlobalItem, new ConditionSO[0], null, null, item);
					list.AddRange(weaponDebuffModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponDebuffModifier> LoadWeaponDebuffModifiersFromBags(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<BagInstance>> weaponsAndContainingBags)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!weaponsAndContainingBags.ContainsKey(item))
			{
				continue;
			}
			foreach (BagInstance item2 in weaponsAndContainingBags[item])
			{
				BagSO bagSO = item2.BagSO;
				if (!(bagSO.Stats == null))
				{
					DebuffSO[] debuffSOs = bagSO.Stats.DebuffSOs;
					foreach (DebuffSO debuffSO in debuffSOs)
					{
						List<WeaponDebuffModifier> weaponDebuffModifier = GetWeaponDebuffModifier(null, debuffSO, Enums.ItemModifierSourceType.Bag, bagSO.Stats.Conditions, null, null, null, null, null, bagSO, hardLinkFilterToWeapon: true);
						list.AddRange(weaponDebuffModifier);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponDebuffModifier> LoadWeaponDebuffModifiersFromRelics(List<RelicSO> relics)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		foreach (RelicSO relic in relics)
		{
			if (!(relic.GlobalStats == null) && relic.GlobalStats.DebuffSOs != null)
			{
				DebuffSO[] debuffSOs = relic.GlobalStats.DebuffSOs;
				foreach (DebuffSO debuffSO in debuffSOs)
				{
					List<WeaponDebuffModifier> weaponDebuffModifier = GetWeaponDebuffModifier(null, debuffSO, Enums.ItemModifierSourceType.Relic, relic.Conditions, null, null, null, relic);
					list.AddRange(weaponDebuffModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponDebuffModifier> LoadWeaponDebuffModifiersFromTalents(List<TalentSO> talents)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		foreach (TalentSO talent in talents)
		{
			if (!(talent.Stats == null) && talent.Stats.DebuffSOs != null)
			{
				DebuffSO[] debuffSOs = talent.Stats.DebuffSOs;
				foreach (DebuffSO debuffSO in debuffSOs)
				{
					List<WeaponDebuffModifier> weaponDebuffModifier = GetWeaponDebuffModifier(null, debuffSO, Enums.ItemModifierSourceType.Talent, talent.Stats.Conditions, null, null, null, null, talent);
					list.AddRange(weaponDebuffModifier);
				}
			}
		}
		return list;
	}

	private static List<WeaponDebuffModifier> LoadWeaponDebuffModifiersFromStarredItems(List<WeaponInstance> weaponsInBackpack, Dictionary<WeaponInstance, List<ItemInstance>> linkedWeaponsAndItemInstanceSets)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (!linkedWeaponsAndItemInstanceSets.ContainsKey(item))
			{
				continue;
			}
			foreach (ItemInstance item2 in linkedWeaponsAndItemInstanceSets[item])
			{
				if (item2.StarDebuffSOs != null)
				{
					DebuffSO[] starDebuffSOs = item2.StarDebuffSOs;
					foreach (DebuffSO debuffSO in starDebuffSOs)
					{
						List<WeaponDebuffModifier> weaponDebuffModifier = GetWeaponDebuffModifier(item, debuffSO, Enums.ItemModifierSourceType.ConnectedItem, item2.StarWeaponConditions, null, item2, null, null, null, null, hardLinkFilterToWeapon: true);
						list.AddRange(weaponDebuffModifier);
					}
				}
			}
		}
		return list;
	}

	private static List<WeaponDebuffModifier> GetWeaponDebuffModifier(WeaponInstance weaponInBackpack, DebuffSO debuffSO, Enums.ItemModifierSourceType itemModifierSourceType, ConditionSO[] conditionSOs, WeaponInstance connectedWeapon = null, ItemInstance connectedItem = null, ItemInstance globalItem = null, RelicSO relicSO = null, TalentSO talentSO = null, BagSO bagSO = null, bool hardLinkFilterToWeapon = false, ItemInstance weaponFilterItem = null)
	{
		List<WeaponDebuffModifier> list = new List<WeaponDebuffModifier>();
		if (!CanCreateValidWeaponModifier(itemModifierSourceType, connectedWeapon, connectedItem, globalItem, relicSO, talentSO, bagSO, weaponFilterItem))
		{
			Debug.LogError($"A valid WeaponStatModifier cannot be created for weapon with guid {weaponInBackpack?.Guid} and ItemModifierSourceType {itemModifierSourceType}");
			return list;
		}
		WeaponDebuffModifier weaponDebuffModifier = new WeaponDebuffModifier
		{
			Source = new ItemModifierSource
			{
				WeaponModifierSourceType = itemModifierSourceType,
				ConnectedWeapon = connectedWeapon,
				ConnectedItem = connectedItem,
				GlobalItem = globalItem,
				Relic = relicSO,
				Talent = talentSO,
				Bag = bagSO,
				ConditionalStatsItem = weaponFilterItem
			},
			DebuffSO = debuffSO
		};
		if (hardLinkFilterToWeapon)
		{
			AddHardTargetWeaponEffectFilter(weaponDebuffModifier, weaponInBackpack);
		}
		if (!conditionSOs.Any())
		{
			list.Add(weaponDebuffModifier);
			return list;
		}
		WeaponDebuffModifier weaponDebuffModifier2 = WeaponDebuffModifier.Clone(weaponDebuffModifier);
		ItemModifierFilter itemModifierFilter = new ItemModifierFilter
		{
			Conditions = GetConditionsFromSOs(conditionSOs),
			HardTargetWeaponInstance = weaponInBackpack
		};
		weaponDebuffModifier2.ItemModifierFilter = itemModifierFilter;
		list.Add(weaponDebuffModifier2);
		return list;
	}

	private static List<Condition> GetConditionsFromSOs(ConditionSO[] conditionSOs)
	{
		List<Condition> list = new List<Condition>();
		foreach (ConditionSO conditionSO in conditionSOs)
		{
			list.Add(new Condition(conditionSO));
		}
		return list;
	}

	private static void AddHardTargetWeaponEffectFilter(WeaponDebuffModifier WeaponDebuffModifier, WeaponInstance weaponInstance)
	{
		ItemModifierFilter itemModifierFilter = new ItemModifierFilter
		{
			HardTargetWeaponInstance = weaponInstance
		};
		WeaponDebuffModifier.ItemModifierFilter = itemModifierFilter;
	}

	public static Dictionary<Enums.ItemStatType, List<ItemStatModifier>> CreateComplexItemStatDictionary()
	{
		Dictionary<Enums.ItemStatType, List<ItemStatModifier>> dictionary = new Dictionary<Enums.ItemStatType, List<ItemStatModifier>>();
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			dictionary.Add(value, new List<ItemStatModifier>());
		}
		return dictionary;
	}

	public static Dictionary<Enums.ItemStatType, float> CreateItemStatDictionary()
	{
		Dictionary<Enums.ItemStatType, float> dictionary = new Dictionary<Enums.ItemStatType, float>();
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			dictionary.Add(value, 0f);
		}
		return dictionary;
	}

	public static Dictionary<Enums.DamageType, float> CreateDamageTypeDictionary()
	{
		Dictionary<Enums.DamageType, float> dictionary = new Dictionary<Enums.DamageType, float>();
		foreach (Enums.DamageType value in Enum.GetValues(typeof(Enums.DamageType)))
		{
			dictionary.Add(value, 0f);
		}
		return dictionary;
	}

	public static Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> CreateComplexDamageTypeDictionary()
	{
		Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> dictionary = new Dictionary<Enums.DamageType, List<DamageTypeValueModifier>>();
		foreach (Enums.DamageType value in Enum.GetValues(typeof(Enums.DamageType)))
		{
			dictionary.Add(value, new List<DamageTypeValueModifier>());
		}
		return dictionary;
	}

	public static Dictionary<Enums.WeaponStatType, float> CreateWeaponStatDictionary()
	{
		Dictionary<Enums.WeaponStatType, float> dictionary = new Dictionary<Enums.WeaponStatType, float>();
		foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
		{
			dictionary.Add(value, 0f);
		}
		return dictionary;
	}

	public static Dictionary<T1, T2> TransformSerializeDictionaryBaseToDictionary<T1, T2>(SerializableDictionaryBase<T1, T2> source)
	{
		Dictionary<T1, T2> dictionary = new Dictionary<T1, T2>();
		foreach (KeyValuePair<T1, T2> item in source)
		{
			dictionary.Add(item.Key, item.Value);
		}
		return dictionary;
	}

	public static bool TryGetWeaponStatTypeFromItemStatType(Enums.ItemStatType itemStatType, out Enums.WeaponStatType weaponStatType)
	{
		switch (itemStatType)
		{
		case Enums.ItemStatType.CritChancePercentage:
			weaponStatType = Enums.WeaponStatType.CritChancePercentage;
			return true;
		case Enums.ItemStatType.CritMultiplier:
			weaponStatType = Enums.WeaponStatType.CritMultiplier;
			return true;
		case Enums.ItemStatType.DamagePercentage:
			weaponStatType = Enums.WeaponStatType.DamagePercentage;
			return true;
		case Enums.ItemStatType.CooldownTime:
			weaponStatType = Enums.WeaponStatType.CooldownTime;
			return true;
		case Enums.ItemStatType.WeaponRange:
			weaponStatType = Enums.WeaponStatType.WeaponRange;
			return true;
		case Enums.ItemStatType.Penetrating:
			weaponStatType = Enums.WeaponStatType.Penetrating;
			return true;
		case Enums.ItemStatType.LifeDrainPercentage:
			weaponStatType = Enums.WeaponStatType.LifeDrainPercentage;
			return true;
		case Enums.ItemStatType.ProjectileCount:
			weaponStatType = Enums.WeaponStatType.ProjectileCount;
			return true;
		case Enums.ItemStatType.ProjectileSpeed:
			weaponStatType = Enums.WeaponStatType.ProjectileSpeed;
			return true;
		case Enums.ItemStatType.CooldownReductionPercentage:
			weaponStatType = Enums.WeaponStatType.CooldownReductionPercentage;
			return true;
		case Enums.ItemStatType.ExplosionSizePercentage:
			weaponStatType = Enums.WeaponStatType.ExplosionSizePercentage;
			return true;
		case Enums.ItemStatType.ProjectileSizePercentage:
			weaponStatType = Enums.WeaponStatType.ProjectileSizePercentage;
			return true;
		case Enums.ItemStatType.StunChancePercentage:
			weaponStatType = Enums.WeaponStatType.StunChancePercentage;
			return true;
		case Enums.ItemStatType.FlatDamage:
			weaponStatType = Enums.WeaponStatType.FlatDamage;
			return true;
		case Enums.ItemStatType.ProjectileDuration:
			weaponStatType = Enums.WeaponStatType.ProjectileDuration;
			return true;
		default:
			weaponStatType = Enums.WeaponStatType.ProjectileSpeed;
			return false;
		}
	}

	public static bool TryGetItemStatTypeFromWeaponStatType(Enums.WeaponStatType weaponStatType, out Enums.ItemStatType itemStatType)
	{
		switch (weaponStatType)
		{
		case Enums.WeaponStatType.CritChancePercentage:
			itemStatType = Enums.ItemStatType.CritChancePercentage;
			return true;
		case Enums.WeaponStatType.CritMultiplier:
			itemStatType = Enums.ItemStatType.CritMultiplier;
			return true;
		case Enums.WeaponStatType.DamagePercentage:
			itemStatType = Enums.ItemStatType.DamagePercentage;
			return true;
		case Enums.WeaponStatType.CooldownTime:
			itemStatType = Enums.ItemStatType.CooldownTime;
			return true;
		case Enums.WeaponStatType.WeaponRange:
			itemStatType = Enums.ItemStatType.WeaponRange;
			return true;
		case Enums.WeaponStatType.Penetrating:
			itemStatType = Enums.ItemStatType.Penetrating;
			return true;
		case Enums.WeaponStatType.LifeDrainPercentage:
			itemStatType = Enums.ItemStatType.LifeDrainPercentage;
			return true;
		case Enums.WeaponStatType.ProjectileCount:
			itemStatType = Enums.ItemStatType.ProjectileCount;
			return true;
		case Enums.WeaponStatType.ProjectileSpeed:
			itemStatType = Enums.ItemStatType.ProjectileSpeed;
			return true;
		case Enums.WeaponStatType.CooldownReductionPercentage:
			itemStatType = Enums.ItemStatType.CooldownReductionPercentage;
			return true;
		case Enums.WeaponStatType.ExplosionSizePercentage:
			itemStatType = Enums.ItemStatType.ExplosionSizePercentage;
			return true;
		case Enums.WeaponStatType.ProjectileSizePercentage:
			itemStatType = Enums.ItemStatType.ProjectileSizePercentage;
			return true;
		case Enums.WeaponStatType.StunChancePercentage:
			itemStatType = Enums.ItemStatType.StunChancePercentage;
			return true;
		case Enums.WeaponStatType.FlatDamage:
			itemStatType = Enums.ItemStatType.FlatDamage;
			return true;
		case Enums.WeaponStatType.ProjectileDuration:
			itemStatType = Enums.ItemStatType.ProjectileDuration;
			return true;
		default:
			itemStatType = Enums.ItemStatType.ProjectileSpeed;
			return false;
		}
	}

	public static bool ItemFitsFilter(WeaponInstance weaponInstance, ItemModifierFilter filter)
	{
		if (filter == null)
		{
			return true;
		}
		bool flag = true;
		if (filter.HardTargetWeaponInstance != null)
		{
			flag = weaponInstance.Guid == filter.HardTargetWeaponInstance.Guid;
		}
		bool flag2 = filter.Conditions.All((Condition c) => c.IsConditionSatisfied(weaponInstance));
		return flag && flag2;
	}

	internal static List<WeaponInstance> GetWeaponsThatFitFilters(ConditionSO[] conditions, List<WeaponInstance> weaponsInBackpack)
	{
		List<WeaponInstance> list = new List<WeaponInstance>();
		ItemModifierFilter filter = new ItemModifierFilter
		{
			Conditions = GetConditionsFromSOs(conditions)
		};
		foreach (WeaponInstance item in weaponsInBackpack)
		{
			if (ItemFitsFilter(item, filter))
			{
				list.Add(item);
			}
		}
		return list;
	}
}
