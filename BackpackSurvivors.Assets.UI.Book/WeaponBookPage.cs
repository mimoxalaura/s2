using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class WeaponBookPage : BookPage
{
	[SerializeField]
	private CollectionWeaponUI _prefab;

	[SerializeField]
	private WeaponDetailPage _detailPage;

	private List<CollectionWeaponUI> _availableCollectionWeaponUIItems;

	internal override void InitPage()
	{
		base.InitPage();
		_availableCollectionWeaponUIItems = new List<CollectionWeaponUI>();
		for (int num = base.ContentLeftContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.ContentLeftContainer.GetChild(num).gameObject);
		}
		foreach (WeaponSO item in from x in GameDatabaseHelper.GetWeapons()
			orderby x.ItemRarity
			select x)
		{
			CollectionWeaponUI collectionWeaponUI = Object.Instantiate(_prefab, base.ContentLeftContainer);
			bool unlocked = SingletonController<CollectionController>.Instance.IsWeaponUnlocked(item.Id);
			collectionWeaponUI.Init(item, unlocked, interactable: true);
			collectionWeaponUI.OnClick += CollectionWeaponUI_OnClick;
			_availableCollectionWeaponUIItems.Add(collectionWeaponUI);
		}
	}

	private void CollectionWeaponUI_OnClick(object sender, WeaponCollectionSelectedEventArgs e)
	{
		_detailPage.InitDetailPage(e.Weapon);
		HighlightSelectedCollectionItem(e.CollectionWeaponUI);
	}

	private void HighlightSelectedCollectionItem(CollectionWeaponUI sender)
	{
		foreach (CollectionWeaponUI availableCollectionWeaponUIItem in _availableCollectionWeaponUIItems)
		{
			availableCollectionWeaponUIItem.ToggleSelect(selected: false);
		}
		sender.ToggleSelect(selected: true);
	}
}
