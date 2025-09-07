using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class RelicBookPage : BookPage
{
	[SerializeField]
	private CollectionRelicUI _prefab;

	[SerializeField]
	private RelicDetailPage _detailPage;

	private List<CollectionRelicUI> _availableCollectionItemUIItems;

	internal override void InitPage()
	{
		base.InitPage();
		_availableCollectionItemUIItems = new List<CollectionRelicUI>();
		for (int num = base.ContentLeftContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.ContentLeftContainer.GetChild(num).gameObject);
		}
		foreach (RelicSO item in from x in GameDatabaseHelper.GetRelics()
			orderby x.Rarity
			select x)
		{
			bool unlocked = SingletonController<CollectionController>.Instance.IsRelicUnlocked(item.Id);
			CollectionRelicUI collectionRelicUI = Object.Instantiate(_prefab, base.ContentLeftContainer);
			collectionRelicUI.Init(item, unlocked, interactable: true);
			collectionRelicUI.OnClick += CollectionWeaponUI_OnClick1;
			_availableCollectionItemUIItems.Add(collectionRelicUI);
		}
	}

	private void CollectionWeaponUI_OnClick1(object sender, RelicCollectionSelectedEventArgs e)
	{
		_detailPage.InitDetailPage(e.Relic);
		HighlightSelectedCollectionItem(e.CollectionRelicUI);
	}

	private void HighlightSelectedCollectionItem(CollectionRelicUI sender)
	{
		foreach (CollectionRelicUI availableCollectionItemUIItem in _availableCollectionItemUIItems)
		{
			availableCollectionItemUIItem.ToggleSelect(selected: false);
		}
		sender.ToggleSelect(selected: true);
	}
}
