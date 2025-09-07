using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems.Bag;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class BagBookPage : BookPage
{
	[SerializeField]
	private CollectionBagUI _prefab;

	[SerializeField]
	private BagDetailPage _detailPage;

	private List<CollectionBagUI> _availableCollectionItemUIItems;

	internal override void InitPage()
	{
		base.InitPage();
		_availableCollectionItemUIItems = new List<CollectionBagUI>();
		for (int num = base.ContentLeftContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.ContentLeftContainer.GetChild(num).gameObject);
		}
		foreach (BagSO item in from x in GameDatabaseHelper.GetBags()
			orderby x.ItemRarity
			select x)
		{
			bool unlocked = SingletonController<CollectionController>.Instance.IsBagUnlocked(item.Id);
			CollectionBagUI collectionBagUI = Object.Instantiate(_prefab, base.ContentLeftContainer);
			collectionBagUI.Init(item, unlocked, interactable: true);
			collectionBagUI.OnClick += CollectionWeaponUI_OnClick;
			_availableCollectionItemUIItems.Add(collectionBagUI);
		}
	}

	private void CollectionWeaponUI_OnClick(object sender, BagCollectionSelectedEventArgs e)
	{
		_detailPage.InitDetailPage(e.Bag);
		HighlightSelectedCollectionItem(e.CollectionBagUI);
	}

	private void HighlightSelectedCollectionItem(CollectionBagUI sender)
	{
		foreach (CollectionBagUI availableCollectionItemUIItem in _availableCollectionItemUIItems)
		{
			availableCollectionItemUIItem.ToggleSelect(selected: false);
		}
		sender.ToggleSelect(selected: true);
	}
}
