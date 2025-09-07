using System;
using BackpackSurvivors.ScriptableObjects.Relics;

namespace BackpackSurvivors.UI.Collection.ListItems.Relic;

public class RelicCollectionSelectedEventArgs : EventArgs
{
	public RelicSO Relic { get; set; }

	public CollectionRelicUI CollectionRelicUI { get; set; }

	public RelicCollectionSelectedEventArgs(RelicSO relic, CollectionRelicUI collectionRelicUI)
	{
		Relic = relic;
		CollectionRelicUI = collectionRelicUI;
	}
}
