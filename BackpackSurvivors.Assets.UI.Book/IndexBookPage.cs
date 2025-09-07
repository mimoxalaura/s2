using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Book;
using UnityEngine;

namespace BackpackSurvivors.Assets.UI.Book;

internal class IndexBookPage : BookPage
{
	[SerializeField]
	private SerializableDictionaryBase<Enums.CollectionMetrics, CollectionBookChapterButton> _chapterButtons;

	[SerializeField]
	private IndexDetailPage _indexDetailPage;

	internal override void InitPage()
	{
		base.InitPage();
		foreach (Enums.CollectionMetrics value in Enum.GetValues(typeof(Enums.CollectionMetrics)))
		{
			int currentKnown = 0;
			int total = 0;
			switch (value)
			{
			case Enums.CollectionMetrics.Weapons:
				total = SingletonController<CollectionController>.Instance.AvailableWeaponsUnlockedStates.Count();
				currentKnown = SingletonController<CollectionController>.Instance.AvailableWeaponsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value);
				break;
			case Enums.CollectionMetrics.Items:
				total = SingletonController<CollectionController>.Instance.AvailableItemsUnlockedStates.Count();
				currentKnown = SingletonController<CollectionController>.Instance.AvailableItemsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value);
				break;
			case Enums.CollectionMetrics.Bags:
				total = SingletonController<CollectionController>.Instance.AvailableBagsUnlockedStates.Count();
				currentKnown = SingletonController<CollectionController>.Instance.AvailableBagsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value);
				break;
			case Enums.CollectionMetrics.Relics:
				total = SingletonController<CollectionController>.Instance.AvailableRelicsUnlockedStates.Count();
				currentKnown = SingletonController<CollectionController>.Instance.AvailableRelicsUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value);
				break;
			case Enums.CollectionMetrics.Recipes:
				total = SingletonController<CollectionController>.Instance.AvailableMergableUnlockedStates.Count();
				currentKnown = SingletonController<CollectionController>.Instance.AvailableMergableUnlockedStates.Count((KeyValuePair<int, bool> x) => x.Value);
				break;
			case Enums.CollectionMetrics.Enemies:
				total = SingletonController<CollectionController>.Instance.AvailableEnemiesUnlockedStates.Count();
				currentKnown = SingletonController<CollectionController>.Instance.AvailableEnemiesUnlockedStates.Count((KeyValuePair<EnemySO, bool> x) => x.Value);
				break;
			}
			if (_chapterButtons.ContainsKey(value))
			{
				_chapterButtons[value].Init(currentKnown, total);
			}
		}
		_indexDetailPage.InitDetailPage();
	}
}
