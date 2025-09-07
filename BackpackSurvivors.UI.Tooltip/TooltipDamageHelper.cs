using System;
using System.Linq;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Tooltip;

public static class TooltipDamageHelper
{
	public static string CreateLine(WeaponInstance weaponInstance)
	{
		float baseMinDamage = weaponInstance.DamageInstance.BaseMinDamage;
		float calculatedMinDamage = weaponInstance.DamageInstance.CalculatedMinDamage;
		string colorForTooltipValues = TooltipHelperBase.GetColorForTooltipValues(baseMinDamage, calculatedMinDamage);
		return string.Concat(string.Concat(string.Empty + $"<size=28><color={colorForTooltipValues}>{(int)weaponInstance.DamageInstance.CalculatedMinDamage}-{(int)weaponInstance.DamageInstance.CalculatedMaxDamage}</color></size>", Environment.NewLine), GetSuffix(weaponInstance));
	}

	public static string CreateLine(WeaponSO weaponSO)
	{
		float num = (int)weaponSO.Damage.BaseMinDamage;
		string colorForTooltipValues = TooltipHelperBase.GetColorForTooltipValues(num, num);
		return string.Concat(string.Concat(string.Empty + $"<size=28><color={colorForTooltipValues}>{(int)weaponSO.Damage.BaseMinDamage}-{(int)weaponSO.Damage.BaseMaxDamage}</color></size>", Environment.NewLine), GetSuffix(weaponSO));
	}

	private static string GetSuffix(WeaponInstance weaponInstance)
	{
		string text = string.Empty;
		Enum[] array = weaponInstance.DamageInstance.CalculatedDamageType.GetUniqueFlags().ToArray();
		if (array.Length == Enum.GetValues(typeof(Enums.DamageType)).Length - 1)
		{
			text = string.Empty;
		}
		for (int i = 0; i < array.Count(); i++)
		{
			text = text + "<sprite name=\"" + StringHelper.GetSpriteValue((Enums.DamageType)(object)array[i]) + "\"> " + TooltipHelperBase.GetDamageTypeString((Enums.DamageType)(object)array[i]);
			if (i < array.Count() - 1)
			{
				text += " and ";
			}
		}
		return "<size=14>" + text + " DAMAGE</size>";
	}

	private static string GetSuffix(WeaponSO weaponSO)
	{
		_ = weaponSO.Damage.BaseMinDamage;
		return "<size=14>" + TooltipHelperBase.GetDamageTypeString(weaponSO.Damage.BaseDamageType) + " DAMAGE</size>";
	}
}
