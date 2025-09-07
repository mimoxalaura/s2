using System;
using System.Linq;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[Serializable]
[CreateAssetMenu(fileName = "WeaponFilter", menuName = "Game/Items/Filter ", order = 1)]
public class WeaponFilterSO : ScriptableObject
{
	[SerializeField]
	public Enums.DamageType DamageType;

	[SerializeField]
	public Enums.WeaponType WeaponType;

	[SerializeField]
	public string Descriptor => "Only affects " + GetWeaponTypeDescriptor(WeaponType) + " " + GetDamageTypeDescriptor(DamageType) + " weapons";

	private string GetWeaponTypeDescriptor(Enums.WeaponType weaponType)
	{
		string text = string.Empty;
		Enum[] array = weaponType.GetUniqueFlags().ToArray();
		if (array.Length == Enum.GetValues(typeof(Enums.WeaponType)).Length - 1)
		{
			return string.Empty;
		}
		for (int i = 0; i < array.Count(); i++)
		{
			text += $"{array[i]}";
			if (i < array.Count() - 1)
			{
				text += " or ";
			}
		}
		return text;
	}

	private string GetDamageTypeDescriptor(Enums.DamageType damageType)
	{
		string text = string.Empty;
		Enum[] array = damageType.GetUniqueFlags().ToArray();
		if (array.Length == Enum.GetValues(typeof(Enums.DamageType)).Length - 1)
		{
			return string.Empty;
		}
		for (int i = 0; i < array.Count(); i++)
		{
			text += $"{array[i]}";
			if (i < array.Count() - 1)
			{
				text += " or ";
			}
		}
		return text;
	}
}
