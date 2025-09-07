using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Adventure.Interfaces;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Items;

public class WeaponInstance : BaseItemInstance, IDPSLoggagable
{
	private WeaponSO _weaponSO;

	private Dictionary<Enums.WeaponStatType, float> _calculatedStats;

	public WeaponSO BaseWeaponSO => _weaponSO;

	public Enums.PlaceableWeaponSubtype WeaponSubtype => _weaponSO.WeaponSubtype;

	public Enums.PlaceableTag CombinedPlaceableTags => EnumHelper.GetCombinedPlaceableTags(this);

	WeaponSO IDPSLoggagable.WeaponSO => _weaponSO;

	float IDPSLoggagable.ActiveSinceTime { get; set; }

	public List<WeaponStatModifier> WeaponModifiers { get; private set; }

	public List<DamageModifier> DamageModifiers { get; private set; }

	public List<WeaponAttackEffectModifier> WeaponAttackEffectModifiers { get; set; }

	public List<WeaponStatModifier> WeaponStatModifiersForStarredSlot { get; set; }

	public List<WeaponAttackEffect> WeaponAttackEffects { get; private set; }

	public List<DebuffHandler> WeaponAttackDebuffHandlers { get; private set; }

	public DamageInstance DamageInstance { get; private set; }

	public string Name => _weaponSO.Name;

	public string Description => _weaponSO.Description;

	public Sprite Icon => _weaponSO.Icon;

	public Enums.PlaceableWeaponSubtype PlaceableWeaponSubtype => _weaponSO.WeaponSubtype;

	public Enums.PlaceableRarity ItemRarity => _weaponSO.ItemRarity;

	public WeaponAttack WeaponAttackPrefab => _weaponSO.WeaponAttackPrefab;

	public Enums.WeaponType BaseWeaponType => _weaponSO.WeaponType;

	public Enums.AttackTargetingType BaseAttackType => _weaponSO.AttackType;

	public Dictionary<Enums.ItemStatType, float> BaseStarStatValues { get; private set; }

	public Dictionary<Enums.DamageType, float> BaseStarDamageTypeValues { get; private set; }

	public WeaponDamageCalculationOverride[] BaseStarWeaponDamageCalculationOverrides
	{
		get
		{
			if (!_weaponSO.HasStarStats())
			{
				return new WeaponDamageCalculationOverride[0];
			}
			return _weaponSO.StarStats.WeaponDamageCalculationOverrides;
		}
	}

	public Dictionary<Enums.WeaponStatType, float> BaseStatValues { get; private set; }

	public Dictionary<Enums.DamageType, float> BaseDamageTypeValues { get; private set; }

	public WeaponStatTypeCalculationOverride[] BaseWeaponStatTypeCalculationOverrides => _weaponSO.Stats.WeaponStatTypeCalculationOverrides;

	public WeaponDamageCalculationOverride[] BaseWeaponDamageCalculationOverrides => _weaponSO.Stats.WeaponDamageCalculationOverrides;

	internal Dictionary<Enums.WeaponStatType, float> CalculatedStats => _calculatedStats;

	public ConditionSO[] BaseStarConditions
	{
		get
		{
			if (!_weaponSO.HasStarStats())
			{
				return new ConditionSO[0];
			}
			return _weaponSO.StarStats.Conditions;
		}
	}

	public WeaponInstance(WeaponSO weaponSO)
	{
		_weaponSO = weaponSO;
		SetBaseItemInstance(weaponSO);
		ResetStats();
	}

	internal void ResetStats()
	{
		ConstructCalculatedStats();
		WeaponModifiers = new List<WeaponStatModifier>();
		DamageModifiers = new List<DamageModifier>();
		WeaponAttackDebuffHandlers = new List<DebuffHandler>();
		DamageInstance = new DamageInstance(_weaponSO.Damage);
		BaseStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_weaponSO.Stats.StatValues);
		BaseDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_weaponSO.Stats.DamageTypeValues);
		if (_weaponSO.StarStats == null)
		{
			BaseStarStatValues = StatCalculator.CreateItemStatDictionary();
			BaseStarDamageTypeValues = StatCalculator.CreateDamageTypeDictionary();
		}
		else
		{
			BaseStarStatValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_weaponSO.StarStats.StatValues);
			BaseStarDamageTypeValues = DictionaryHelper.SetupDictionaryFromSerialableDictionary(_weaponSO.StarStats.DamageTypeValues);
		}
		ConstructWeaponAttackEffects();
		CalculateDebuffEffects();
		CalculateAttacksEffects();
	}

	public float GetCalculatedStat(Enums.WeaponStatType statType)
	{
		if (_calculatedStats.ContainsKey(statType))
		{
			return _calculatedStats[statType];
		}
		return 0f;
	}

	public void ScaleWeaponDamage(float damageScale)
	{
		DamageInstance.ScaleDamage(damageScale);
	}

	private void ConstructWeaponAttackEffects()
	{
		WeaponAttackEffects = new List<WeaponAttackEffect>();
		WeaponAttackEffectModifiers = new List<WeaponAttackEffectModifier>();
		if (_weaponSO.WeaponAttackEffects != null)
		{
			WeaponAttackEffects = _weaponSO.WeaponAttackEffects.ToList();
		}
	}

	private void ConstructCalculatedStats()
	{
		_calculatedStats = StatCalculator.CreateWeaponStatDictionary();
		if (_weaponSO.Stats.StatValues.Count > 0)
		{
			foreach (KeyValuePair<Enums.WeaponStatType, float> statValue in _weaponSO.Stats.StatValues)
			{
				_calculatedStats[statValue.Key] += statValue.Value;
			}
		}
		SetDefaultValues(_calculatedStats);
	}

	private void SetDefaultValues(Dictionary<Enums.WeaponStatType, float> calculatedStats)
	{
		AddDefaultIfNeeded(calculatedStats, Enums.WeaponStatType.DamagePercentage, 1f);
		AddDefaultIfNeeded(calculatedStats, Enums.WeaponStatType.ExplosionSizePercentage, 1f);
		AddDefaultIfNeeded(calculatedStats, Enums.WeaponStatType.ProjectileSizePercentage, 1f);
	}

	private void AddDefaultIfNeeded(Dictionary<Enums.WeaponStatType, float> calculatedStats, Enums.WeaponStatType weaponStatType, float defaultValue)
	{
		calculatedStats.TryGetValue(weaponStatType, out var value);
		if (!(value > 0f))
		{
			calculatedStats[weaponStatType] = defaultValue;
		}
	}

	public void CalculateDebuffEffects()
	{
		DebuffSO[] debuffAttackEffects = _weaponSO.DebuffAttackEffects;
		foreach (DebuffSO debuffSO in debuffAttackEffects)
		{
			DebuffHandler debuffHandler = new DebuffHandler();
			debuffHandler.Init(debuffSO);
			WeaponAttackDebuffHandlers.Add(debuffHandler);
		}
	}

	public void CalculateAttacksEffects()
	{
		foreach (WeaponAttackEffect weaponAttackEffect in WeaponAttackEffects)
		{
			weaponAttackEffect.HandlerOnTrigger.Init(_weaponSO.WeaponAttackPrefab, WeaponAttackDebuffHandlers, this);
		}
	}

	private bool CheckIfValueOverridesAnotherAndOverride(WeaponStatModifier weaponStatModifier)
	{
		if (weaponStatModifier.WeaponStatType == Enums.WeaponStatType.ProjectileCount && BaseWeaponSO.WeaponType == Enums.WeaponType.Melee)
		{
			float calculatedBonus = weaponStatModifier.CalculatedBonus;
			if (GetCalculatedStat(Enums.WeaponStatType.ProjectileCount) + calculatedBonus != 1f)
			{
				_calculatedStats[weaponStatModifier.WeaponStatType] = 1f;
				WeaponModifiers.Add(weaponStatModifier);
				return true;
			}
		}
		return false;
	}

	public void AddModifier(WeaponStatModifier weaponStatModifier)
	{
		if (!CheckIfValueOverridesAnotherAndOverride(weaponStatModifier))
		{
			WeaponModifiers.Add(weaponStatModifier);
		}
	}

	private Enums.PlaceableTag GetCombinedPlaceableTags()
	{
		long num = 0L;
		Enums.PlaceableTag placeableTag = EnumHelper.WeaponTypesToPlaceableTags(BaseWeaponType);
		long num2 = num | (long)placeableTag;
		Enums.PlaceableTag placeableTag2 = EnumHelper.WeaponSubtypeToPlaceableTag(WeaponSubtype);
		long num3 = num2 | (long)placeableTag2;
		Enums.PlaceableTag placeableTag3 = EnumHelper.DamageTypesToPlaceableTags(DamageInstance.CalculatedDamageType);
		return (Enums.PlaceableTag)(num3 | (long)placeableTag3);
	}

	public void CalculateStats()
	{
		foreach (object value in Enum.GetValues(typeof(Enums.WeaponStatType)))
		{
			Enums.WeaponStatType weaponStatType = (Enums.WeaponStatType)value;
			float totalBonus = WeaponModifiers.Where((WeaponStatModifier wm) => wm.WeaponStatType == weaponStatType).Sum((WeaponStatModifier wm) => wm.CalculatedBonus);
			ApplyBonus(weaponStatType, totalBonus);
		}
	}

	private void ApplyBonus(Enums.WeaponStatType weaponStatType, float totalBonus)
	{
		switch (weaponStatType)
		{
		case Enums.WeaponStatType.CritChancePercentage:
		case Enums.WeaponStatType.CritMultiplier:
		case Enums.WeaponStatType.Penetrating:
		case Enums.WeaponStatType.LifeDrainPercentage:
		case Enums.WeaponStatType.ProjectileCount:
		case Enums.WeaponStatType.StunChancePercentage:
			_calculatedStats[weaponStatType] += totalBonus;
			break;
		case Enums.WeaponStatType.DamagePercentage:
		case Enums.WeaponStatType.WeaponRange:
		case Enums.WeaponStatType.ExplosionSizePercentage:
		case Enums.WeaponStatType.ProjectileSpeed:
		case Enums.WeaponStatType.ProjectileSizePercentage:
		case Enums.WeaponStatType.ProjectileDuration:
			_calculatedStats[weaponStatType] *= totalBonus;
			break;
		case Enums.WeaponStatType.FlatDamage:
			DamageInstance.CalculatedMinDamage += totalBonus;
			DamageInstance.CalculatedMaxDamage += totalBonus;
			DamageInstance.CalculatedMinDamage = Mathf.Clamp(DamageInstance.CalculatedMinDamage, 0f, 9999f);
			DamageInstance.CalculatedMaxDamage = Mathf.Clamp(DamageInstance.CalculatedMaxDamage, 0f, 9999f);
			break;
		case Enums.WeaponStatType.CooldownReductionPercentage:
		{
			float num = 1f - totalBonus;
			float value = _calculatedStats[Enums.WeaponStatType.CooldownTime] * num;
			float min = _weaponSO.Stats.StatValues.TryGet(Enums.WeaponStatType.CooldownTime, 1f) * 0.25f;
			value = Math.Clamp(value, min, 999f);
			_calculatedStats[Enums.WeaponStatType.CooldownTime] = value;
			break;
		}
		default:
			Debug.LogWarning(string.Format("Enum {0} is not handled in {1}.{2}", weaponStatType, "WeaponInstance", "ApplyBonus"));
			break;
		case Enums.WeaponStatType.CooldownTime:
			break;
		}
	}

	public void AddWeaponAttackEffect(WeaponAttackEffectModifier weaponAttackEffectModifier)
	{
		if (!WeaponAttackEffects.Any((WeaponAttackEffect x) => x.Id == weaponAttackEffectModifier.WeaponAttackEffect.Id))
		{
			WeaponAttackEffectModifiers.Add(weaponAttackEffectModifier);
			WeaponAttackEffects.Add(weaponAttackEffectModifier.WeaponAttackEffect);
		}
	}

	internal void AddWeaponDebuff(WeaponDebuffModifier weaponDebuffModifier)
	{
		if (!WeaponAttackDebuffHandlers.Any((DebuffHandler x) => x.DebuffSO.Id == weaponDebuffModifier.DebuffSO.Id))
		{
			DebuffHandler debuffHandler = new DebuffHandler();
			debuffHandler.Init(weaponDebuffModifier.DebuffSO);
			WeaponAttackDebuffHandlers.Add(debuffHandler);
		}
	}

	public void SetModifier(WeaponStatModifier weaponStatModifier)
	{
		_calculatedStats[weaponStatModifier.WeaponStatType] = weaponStatModifier.CalculatedBonus;
		WeaponModifiers.Add(weaponStatModifier);
	}

	public void SetCalculatedStat(Enums.WeaponStatType stat, float statValue)
	{
		_calculatedStats[stat] = statValue;
	}

	public void AddDamageModifiers(List<DamageModifier> damageModifiers)
	{
		float num = damageModifiers.Sum((DamageModifier d) => d.CalculatedBonus);
		DamageInstance.CalculatedMinDamage *= num;
		DamageInstance.CalculatedMaxDamage *= num;
		DamageModifiers.AddRange(damageModifiers);
	}

	public void SetMinMaxDamage(float minDamage, float maxDamage)
	{
		DamageInstance.CalculatedMinDamage = minDamage;
		DamageInstance.CalculatedMaxDamage = maxDamage;
	}

	public override bool Equals(object other)
	{
		if (other is WeaponInstance)
		{
			return ((WeaponInstance)other).Guid == base.Guid;
		}
		return base.Equals(other);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	internal bool HasStarStats()
	{
		return BaseWeaponSO.HasStarStats();
	}
}
