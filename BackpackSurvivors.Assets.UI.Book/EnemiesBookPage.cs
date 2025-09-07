using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems.Enemy;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class EnemiesBookPage : BookPage
{
	[SerializeField]
	private CollectionEnemyUI _prefab;

	[SerializeField]
	private EnemyDetailPage _detailPage;

	private List<CollectionEnemyUI> _availableCollectionItemUIEnemies;

	internal override void InitPage()
	{
		base.InitPage();
		_availableCollectionItemUIEnemies = new List<CollectionEnemyUI>();
		for (int num = base.ContentLeftContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.ContentLeftContainer.GetChild(num).gameObject);
		}
		foreach (EnemySO item in from x in GameDatabaseHelper.GetEnemies()
			orderby x.EnemyType
			select x)
		{
			bool unlocked = SingletonController<CollectionController>.Instance.IsEnemyUnlocked(item);
			CollectionEnemyUI collectionEnemyUI = Object.Instantiate(_prefab, base.ContentLeftContainer);
			collectionEnemyUI.Init(item, unlocked, interactable: true);
			collectionEnemyUI.OnClick += CollectionEnemyUI_OnClick;
			_availableCollectionItemUIEnemies.Add(collectionEnemyUI);
		}
	}

	private void CollectionEnemyUI_OnClick(object sender, EnemyCollectionSelectedEventArgs e)
	{
		_detailPage.InitDetailPage(e.Enemy);
		HighlightSelectedCollectionItem(e.CollectionEnemyUI);
	}

	private void HighlightSelectedCollectionItem(CollectionEnemyUI sender)
	{
		foreach (CollectionEnemyUI availableCollectionItemUIEnemy in _availableCollectionItemUIEnemies)
		{
			availableCollectionItemUIEnemy.ToggleSelect(selected: false);
		}
		sender.ToggleSelect(selected: true);
	}
}
