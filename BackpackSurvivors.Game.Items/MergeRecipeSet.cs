using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.Game.Items;

public class MergeRecipeSet
{
	internal BaseItemInstance PrimaryBaseItemInstance;

	internal bool RecipeIsComplete;

	internal MergableSO Recipe { get; private set; }

	internal List<BaseItemInstance> AvailableItemsForMerge { get; private set; } = new List<BaseItemInstance>();

	internal List<BaseItemInstance> AvailableItemsForMergeExcludingPrimaryItem => AvailableItemsForMerge.Where((BaseItemInstance x) => x.Guid != PrimaryBaseItemInstance.Guid).ToList();

	internal void SetData(MergableSO recipe, List<BaseItemInstance> items)
	{
		Recipe = recipe;
		AvailableItemsForMerge = items;
		string primaryItemName = Recipe.Input.FirstOrDefault((MergeableIngredient x) => x.IsPrimary)?.ItemName;
		PrimaryBaseItemInstance = AvailableItemsForMerge.FirstOrDefault((BaseItemInstance x) => x.BaseItemSO.Name == primaryItemName);
	}

	public override int GetHashCode()
	{
		HashCode hashCode = default(HashCode);
		foreach (BaseItemInstance item in AvailableItemsForMerge)
		{
			hashCode.Add(item.Guid);
		}
		return hashCode.ToHashCode();
	}

	public override bool Equals(object other)
	{
		if (!(other is MergeRecipeSet))
		{
			return false;
		}
		return GetHashCode().Equals(other.GetHashCode());
	}
}
