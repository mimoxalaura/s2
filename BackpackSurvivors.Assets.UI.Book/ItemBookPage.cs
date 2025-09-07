using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems.Item;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class ItemBookPage : BookPage
{
	[SerializeField]
	private CollectionItemUI _prefab;

	[SerializeField]
	private ItemDetailPage _detailPage;

	private List<CollectionItemUI> _availableCollectionItemUIItems;

	internal override void InitPage()
	{
		base.InitPage();
		_availableCollectionItemUIItems = new List<CollectionItemUI>();
		for (int num = base.ContentLeftContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.ContentLeftContainer.GetChild(num).gameObject);
		}
		foreach (ItemSO item in from x in GameDatabaseHelper.GetItems()
			orderby x.ItemRarity
			select x)
		{
			bool unlocked = SingletonController<CollectionController>.Instance.IsItemUnlocked(item.Id);
			CollectionItemUI collectionItemUI = Object.Instantiate(_prefab, base.ContentLeftContainer);
			collectionItemUI.Init(item, unlocked, interactable: true);
			collectionItemUI.OnClick += CollectionWeaponUI_OnClick1;
			_availableCollectionItemUIItems.Add(collectionItemUI);
		}
	}

	private void CollectionWeaponUI_OnClick1(object sender, ItemCollectionSelectedEventArgs e)
	{
		_detailPage.InitDetailPage(e.Item);
		HighlightSelectedCollectionItem(e.CollectionItemUI);
	}

	private void HighlightSelectedCollectionItem(CollectionItemUI sender)
	{
		foreach (CollectionItemUI availableCollectionItemUIItem in _availableCollectionItemUIItems)
		{
			availableCollectionItemUIItem.ToggleSelect(selected: false);
		}
		sender.ToggleSelect(selected: true);
	}
}
