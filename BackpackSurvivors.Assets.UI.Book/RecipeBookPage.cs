using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems.Recipe;
using BackpackSurvivors.UI.Merges;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class RecipeBookPage : BookPage
{
	[SerializeField]
	private MergeRecipeVisualItem _prefab;

	[SerializeField]
	private RecipeDetailPage _detailPage;

	private List<MergeRecipeVisualItem> _availableCollectionItemUIItems;

	internal override void InitPage()
	{
		base.InitPage();
		_availableCollectionItemUIItems = new List<MergeRecipeVisualItem>();
		for (int num = base.ContentLeftContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.ContentLeftContainer.GetChild(num).gameObject);
		}
		foreach (MergableSO item in from x in GameDatabaseHelper.GetMergeRecipes()
			orderby x.Output.BaseItem.ItemRarity
			select x)
		{
			bool unlocked = SingletonController<CollectionController>.Instance.IsRecipeUnlocked(item.Id);
			MergeRecipeVisualItem mergeRecipeVisualItem = Object.Instantiate(_prefab, base.ContentLeftContainer);
			mergeRecipeVisualItem.Init(item, unlocked);
			mergeRecipeVisualItem.OnClick += CollectionWeaponUI_OnClick;
			_availableCollectionItemUIItems.Add(mergeRecipeVisualItem);
		}
	}

	private void CollectionWeaponUI_OnClick(object sender, RecipeCollectionSelectedEventArgs e)
	{
		_detailPage.InitDetailPage(e.Recipe);
		HighlightSelectedCollectionItem(e.MergeRecipeVisualItem);
	}

	private void HighlightSelectedCollectionItem(MergeRecipeVisualItem sender)
	{
		foreach (MergeRecipeVisualItem availableCollectionItemUIItem in _availableCollectionItemUIItems)
		{
			availableCollectionItemUIItem.ToggleSelect(selected: false);
		}
		sender.ToggleSelect(selected: true);
	}
}
