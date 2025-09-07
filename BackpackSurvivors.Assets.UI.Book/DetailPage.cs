using System;
using System.Linq;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class DetailPage : MonoBehaviour
{
	internal string CreateTagsString(Enums.PlaceableTag tags, Enums.PlaceableType placeableType)
	{
		string text = string.Empty;
		string[] names = Enum.GetNames(typeof(Enums.PlaceableRarity));
		string[] names2 = Enum.GetNames(typeof(Enums.DamageType));
		string[] names3 = Enum.GetNames(typeof(Enums.PlaceableWeaponSubtype));
		string[] names4 = Enum.GetNames(typeof(Enums.WeaponType));
		foreach (Enum uniqueFlag in tags.GetUniqueFlags())
		{
			if (!names.Contains(uniqueFlag.ToString()))
			{
				continue;
			}
			Enums.PlaceableRarity rarity = Enums.PlaceableRarity.Common;
			foreach (Enums.PlaceableRarity value in Enum.GetValues(typeof(Enums.PlaceableRarity)))
			{
				if (value.ToString() == uniqueFlag.ToString())
				{
					rarity = value;
				}
			}
			string colorHexcodeForRarity = ColorHelper.GetColorHexcodeForRarity(rarity);
			text += $"<color=#{colorHexcodeForRarity}>{uniqueFlag}</color>, ";
		}
		switch (placeableType)
		{
		case Enums.PlaceableType.Weapon:
			text += "Weapon, ";
			break;
		case Enums.PlaceableType.Item:
			text += "Item, ";
			break;
		case Enums.PlaceableType.Bag:
			text += "Bag, ";
			break;
		}
		foreach (Enum uniqueFlag2 in tags.GetUniqueFlags())
		{
			if (names2.Contains(uniqueFlag2.ToString()))
			{
				Enums.DamageType damageType = Enums.DamageType.None;
				foreach (Enums.DamageType value2 in Enum.GetValues(typeof(Enums.DamageType)))
				{
					if (value2.ToString() == uniqueFlag2.ToString())
					{
						damageType = value2;
					}
				}
				string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(damageType);
				text += $"<color={colorStringForDamageType}>{uniqueFlag2}</color>, ";
			}
			else
			{
				if (names.Contains(uniqueFlag2.ToString()) || names3.Contains(uniqueFlag2.ToString()))
				{
					continue;
				}
				if (names4.Contains(uniqueFlag2.ToString()))
				{
					Enums.WeaponType weaponType = Enums.WeaponType.Melee;
					foreach (Enums.WeaponType value3 in Enum.GetValues(typeof(Enums.WeaponType)))
					{
						if (value3.ToString() == uniqueFlag2.ToString())
						{
							weaponType = value3;
						}
					}
					string colorForWeaponType = ColorHelper.GetColorForWeaponType(weaponType);
					text += $"<color={colorForWeaponType}>{uniqueFlag2}</color>, ";
				}
				else
				{
					text += $"{uniqueFlag2}, ";
				}
			}
		}
		if (text.EndsWith(", "))
		{
			text = text.Remove(text.Length - 2, 2);
		}
		return text ?? "";
	}
}
