using System;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.UI.Merges;

namespace BackpackSurvivors.UI.Collection.ListItems.Recipe;

internal class RecipeCollectionSelectedEventArgs : EventArgs
{
	internal MergableSO Recipe { get; set; }

	internal MergeRecipeVisualItem MergeRecipeVisualItem { get; set; }

	internal RecipeCollectionSelectedEventArgs(MergableSO recipe, MergeRecipeVisualItem mergeRecipeVisualItem)
	{
		Recipe = recipe;
		MergeRecipeVisualItem = mergeRecipeVisualItem;
	}
}
