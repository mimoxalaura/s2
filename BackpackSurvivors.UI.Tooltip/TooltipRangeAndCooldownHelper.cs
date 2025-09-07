using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipRangeAndCooldownHelper
{
	private static Dictionary<Enums.ProjectileMovement, string> _customRangeStrings = new Dictionary<Enums.ProjectileMovement, string> { 
	{
		Enums.ProjectileMovement.RotatingAroundStartPosition,
		"Circling range"
	} };

	public static string CreateLine(WeaponInstance weaponInstance)
	{
		string text = string.Empty;
		if (!weaponInstance.BaseWeaponSO.IsPermanentEffect)
		{
			float baseValue = weaponInstance.BaseStatValues[Enums.WeaponStatType.WeaponRange];
			float calculatedStat = weaponInstance.GetCalculatedStat(Enums.WeaponStatType.WeaponRange);
			Enums.ProjectileMovement? projectileMovement = weaponInstance.BaseWeaponSO.WeaponAttackPrefab.ProjectilePrefab?.ProjectileMovement;
			text += GetRangeText(baseValue, calculatedStat, projectileMovement);
			text += Environment.NewLine;
		}
		return text + GetCooldownText(weaponInstance);
	}

	public static string CreateLine(WeaponSO weaponSO)
	{
		string text = string.Empty;
		if (!weaponSO.IsPermanentEffect)
		{
			float num = weaponSO.Stats.StatValues[Enums.WeaponStatType.WeaponRange];
			Enums.ProjectileMovement projectileMovement = weaponSO.WeaponAttackPrefab.ProjectilePrefab.ProjectileMovement;
			text += GetRangeText(num, num, projectileMovement);
			text += Environment.NewLine;
		}
		return text + GetCooldownText(weaponSO);
	}

	private static string GetRangeText(float baseValue, float newValue, Enums.ProjectileMovement? projectileMovement)
	{
		string colorForTooltipValues = TooltipHelperBase.GetColorForTooltipValues(baseValue, newValue);
		string rangeString = GetRangeString(projectileMovement);
		return $"{rangeString} <color={colorForTooltipValues}>{newValue:f2}</color>";
	}

	private static string GetRangeString(Enums.ProjectileMovement? projectileMovement)
	{
		if (!projectileMovement.HasValue)
		{
			return string.Empty;
		}
		if (_customRangeStrings.ContainsKey(projectileMovement.Value))
		{
			return _customRangeStrings[projectileMovement.Value];
		}
		return "Range";
	}

	public static string GetCooldownText(WeaponInstance weaponInstance)
	{
		if (weaponInstance.BaseWeaponSO.IsPermanentEffect)
		{
			return "Permanent";
		}
		float newValue = weaponInstance.BaseStatValues[Enums.WeaponStatType.CooldownTime];
		string colorForTooltipValues = TooltipHelperBase.GetColorForTooltipValues(weaponInstance.GetCalculatedStat(Enums.WeaponStatType.CooldownTime), newValue);
		return "Cooldown <color=" + colorForTooltipValues + ">" + $"{weaponInstance.GetCalculatedStat(Enums.WeaponStatType.CooldownTime):0.00}" + "</color>";
	}

	public static string GetCooldownText(WeaponSO weaponSO)
	{
		if (weaponSO.IsPermanentEffect)
		{
			return "Permanent";
		}
		float num = weaponSO.Stats.StatValues[Enums.WeaponStatType.CooldownTime];
		string colorForTooltipValues = TooltipHelperBase.GetColorForTooltipValues(num, num);
		return "Cooldown <color=" + colorForTooltipValues + ">" + $"{num:0.00}" + "</color>";
	}
}
