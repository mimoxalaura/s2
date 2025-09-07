using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Items;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Mergable", menuName = "Game/Items/Mergable", order = 1)]
internal class MergableSO : ScriptableObject
{
	public string Title;

	public string BasicTitle;

	private GameDatabaseSO gameDatabase;

	[SerializeField]
	public int Id;

	[SerializeField]
	public List<MergeableIngredient> Input;

	[SerializeField]
	public MergeableResult Output;

	private void CreateData()
	{
		try
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			int num = Input.Count() - 1;
			MergeableIngredient mergeableIngredient = Input.FirstOrDefault((MergeableIngredient x) => x.IsPrimary);
			if (Input != null && Input.Any())
			{
				empty += $"{mergeableIngredient.Amount} <color={GetItemColor(mergeableIngredient.BaseItem)}>{mergeableIngredient.ItemName}</color> + ";
				empty2 += $"{mergeableIngredient.ItemName} ({mergeableIngredient.Amount}) + ";
				for (int num2 = 0; num2 <= num; num2++)
				{
					if (!Input[num2].IsPrimary)
					{
						empty += $"{Input[num2].Amount} <color={GetItemColor(Input[num2].BaseItem)}>{Input[num2].ItemName}</color> + ";
						empty2 += $"{Input[num2].ItemName} ({Input[num2].Amount}) + ";
					}
				}
				empty = empty.Substring(0, empty.Length - 3);
				empty2 = empty2.Substring(0, empty2.Length - 3);
				Title = $"{empty} = <color={GetItemColor(Output.BaseItem)}>{Output.ItemName}</color>";
				BasicTitle = empty2 + " = " + Output.ItemName;
			}
			else
			{
				Title = empty;
			}
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"{arg}");
		}
	}

	private Color GetItemColor(BaseItemSO baseItemSO)
	{
		return Color.white;
	}

	private void RenameFile()
	{
	}

	private void SetId()
	{
	}

	internal bool ItemIsPartOfMergable(BaseItemInstance baseItemInstance)
	{
		return Input.Any((MergeableIngredient x) => x.ItemName == baseItemInstance.BaseItemSO.Name);
	}

	internal bool ItemIsNeededInMultiples(BaseItemInstance baseItemInstance)
	{
		if (ItemIsPartOfMergable(baseItemInstance))
		{
			return Input.FirstOrDefault((MergeableIngredient x) => x.ItemName == baseItemInstance.BaseItemSO.Name).Amount > 1;
		}
		return false;
	}

	internal bool IsCompleteMerge(List<BaseItemInstance> items)
	{
		bool result = true;
		foreach (MergeableIngredient item in Input)
		{
			string itemToFind = item.ItemName;
			int amount = item.Amount;
			if (items.Count((BaseItemInstance x) => x.BaseItemSO.Name == itemToFind) < amount)
			{
				result = false;
			}
		}
		return result;
	}

	internal bool IsIncompleteMerge(List<BaseItemInstance> items)
	{
		return CanPotentiallyMerge(items);
	}

	internal bool CanPotentiallyMerge(List<BaseItemInstance> items)
	{
		bool result = true;
		foreach (MergeableIngredient item in Input)
		{
			string itemToFind = item.ItemName;
			_ = item.Amount;
			if (items.Count((BaseItemInstance x) => x.BaseItemSO.Name == itemToFind) < 0)
			{
				result = false;
			}
		}
		return result;
	}

	internal int NumberOfItemsNeeded(string name)
	{
		return Input.FirstOrDefault((MergeableIngredient x) => x.ItemName == name)?.Amount ?? 0;
	}
}
