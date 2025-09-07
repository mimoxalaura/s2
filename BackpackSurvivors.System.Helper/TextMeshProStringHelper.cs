using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BackpackSurvivors.Game.Core;

namespace BackpackSurvivors.System.Helper;

public class TextMeshProStringHelper
{
	public static string ToBold(string original)
	{
		return "<b>" + original + "</b>";
	}

	public static string ToCaps(string original)
	{
		return "<uppercase>" + original + "</uppercase>";
	}

	public static string AddColor(string hexColor, string text)
	{
		return "<color=" + hexColor + ">" + text + "</color>";
	}

	public static string HighlightKeywords(string originalString, string hexColorForHighlight)
	{
		string text = string.Empty;
		string[] array = originalString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			if (IsKeyword(array[i]))
			{
				list.Add("<color=" + hexColorForHighlight + ">" + array[i] + "</color>");
			}
			else
			{
				list.Add(array[i]);
			}
		}
		foreach (string item in list)
		{
			text = text + item + " ";
		}
		return text;
	}

	public static string HighlightElementalKeywords(string originalString)
	{
		string text = string.Empty;
		string[] array = originalString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			Enums.DamageType foundDamageType = Enums.DamageType.None;
			if (IsElementalKeyword(array[i], out foundDamageType) && (foundDamageType != Enums.DamageType.None || foundDamageType != Enums.DamageType.All))
			{
				string colorStringForDamageType = ColorHelper.GetColorStringForDamageType(foundDamageType);
				list.Add("<color=" + colorStringForDamageType + ">" + ToBold(ToCaps(array[i])) + "</color>");
			}
			else
			{
				list.Add(array[i]);
			}
		}
		foreach (string item in list)
		{
			text = text + item + " ";
		}
		return text;
	}

	public static string HighlightDebuffKeywords(string originalString)
	{
		string text = string.Empty;
		string[] array = originalString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			Enums.Debuff.DebuffType foundDebuffType = Enums.Debuff.DebuffType.None;
			if (IsDebuffKeyword(array[i], out foundDebuffType))
			{
				string colorStringForDebuffType = ColorHelper.GetColorStringForDebuffType(foundDebuffType);
				list.Add("<color=" + colorStringForDebuffType + ">" + ToBold(ToCaps(array[i])) + "</color>");
			}
			else
			{
				list.Add(array[i]);
			}
		}
		foreach (string item in list)
		{
			text = text + item + " ";
		}
		return text;
	}

	private static bool IsKeyword(string potentialKeyword)
	{
		if (string.IsNullOrEmpty(potentialKeyword))
		{
			return false;
		}
		string clean = RemoveSpecialCharacters(potentialKeyword);
		if (string.IsNullOrEmpty(clean))
		{
			return false;
		}
		return SingletonController<GameDatabase>.Instance.GameDatabaseSO.Keywords.Any((string x) => x.ToLower() == clean.ToLower());
	}

	private static bool IsElementalKeyword(string potentialKeyword, out Enums.DamageType foundDamageType)
	{
		foundDamageType = Enums.DamageType.None;
		foreach (Enums.DamageType value in Enum.GetValues(typeof(Enums.DamageType)))
		{
			if (value.ToString().ToLower() == potentialKeyword.ToLower())
			{
				switch (value)
				{
				case Enums.DamageType.All:
					return false;
				case Enums.DamageType.None:
					return false;
				default:
					foundDamageType = value;
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsDebuffKeyword(string potentialKeyword, out Enums.Debuff.DebuffType foundDebuffType)
	{
		foundDebuffType = Enums.Debuff.DebuffType.None;
		foreach (Enums.Debuff.DebuffType value in Enum.GetValues(typeof(Enums.Debuff.DebuffType)))
		{
			if (value.ToString().ToLower() == potentialKeyword.ToLower())
			{
				foundDebuffType = value;
				return true;
			}
		}
		return false;
	}

	internal static string HighlightWeaponStatKeywords(string originalString, string hexColorForHighlight)
	{
		string text = string.Empty;
		string[] array = originalString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			Enums.WeaponStatType foundWeaponStatType = Enums.WeaponStatType.WeaponRange;
			if (IsWeaponStatKeyword(array[i], out foundWeaponStatType))
			{
				list.Add("<color=" + hexColorForHighlight + ">" + ToBold(ToCaps(array[i])) + "</color>");
			}
			else
			{
				list.Add(array[i]);
			}
		}
		foreach (string item in list)
		{
			text = text + item + " ";
		}
		return text;
	}

	private static bool IsWeaponStatKeyword(string potentialKeyword, out Enums.WeaponStatType foundWeaponStatType)
	{
		foundWeaponStatType = Enums.WeaponStatType.WeaponRange;
		foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
		{
			if (StringHelper.GetCleanString(value).ToLower() == potentialKeyword.ToLower())
			{
				foundWeaponStatType = value;
				return true;
			}
		}
		return false;
	}

	internal static string HighlightItemStatKeywords(string originalString, string hexColorForHighlight, bool useBrackets = false)
	{
		string text = string.Empty;
		string[] array = originalString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			Enums.ItemStatType foundWeaponStatType = Enums.ItemStatType.WeaponRange;
			if (IsItemStatKeyword(array[i], out foundWeaponStatType) && foundWeaponStatType != Enums.ItemStatType.DamagePercentage)
			{
				if (useBrackets)
				{
					list.Add("<color=" + hexColorForHighlight + ">[" + ToBold(ToCaps(array[i])) + "]</color>");
				}
				else
				{
					list.Add("<color=" + hexColorForHighlight + ">" + ToBold(ToCaps(array[i])) + "</color>");
				}
			}
			else
			{
				list.Add(array[i]);
			}
		}
		foreach (string item in list)
		{
			text = text + item + " ";
		}
		return text;
	}

	private static bool IsItemStatKeyword(string potentialKeyword, out Enums.ItemStatType foundWeaponStatType)
	{
		foundWeaponStatType = Enums.ItemStatType.WeaponRange;
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			if (StringHelper.GetCleanString(value).ToLower() == potentialKeyword.ToLower())
			{
				foundWeaponStatType = value;
				return true;
			}
		}
		return false;
	}

	private static string RemoveSpecialCharacters(string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in str)
		{
			if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	internal static string HighlightTags(string originalString, bool useBrackets = false)
	{
		string text = string.Empty;
		string[] array = originalString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			foreach (Enums.PlaceableTag value in Enum.GetValues(typeof(Enums.PlaceableTag)))
			{
				if (text2.ToString() == value.ToString())
				{
					string color = ColorHelper.GetColor(value);
					text2 = ((!useBrackets) ? (" <color=" + color + ">" + text2 + "</color>") : (" <color=" + color + ">[" + text2 + "]</color>"));
				}
			}
			text = text + " " + text2;
		}
		originalString = text;
		return text;
	}
}
