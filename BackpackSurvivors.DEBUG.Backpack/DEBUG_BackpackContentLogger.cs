using System.Collections.Generic;
using System.Text;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Items;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.DEBUG.Backpack;

public class DEBUG_BackpackContentLogger : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _logText;

	public void Clear()
	{
		_logText.text = string.Empty;
	}

	public void LogBackpack()
	{
		StringBuilder stringBuilder = new StringBuilder();
		BackpackController backpackController = Object.FindObjectOfType<BackpackController>();
		List<WeaponInstance> weaponsFromBackpack = backpackController.GetWeaponsFromBackpack();
		stringBuilder.AppendLine("-= Weapons =-");
		foreach (WeaponInstance item in weaponsFromBackpack)
		{
			stringBuilder.AppendLine(item.Name ?? "");
		}
		stringBuilder.AppendLine();
		List<ItemInstance> itemsFromBackpack = backpackController.GetItemsFromBackpack();
		stringBuilder.AppendLine("-= Items =-");
		foreach (ItemInstance item2 in itemsFromBackpack)
		{
			stringBuilder.AppendLine(item2.Name ?? "");
		}
		_logText.text = stringBuilder.ToString();
	}
}
