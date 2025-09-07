using System;
using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.UI.Collection.ListItems.Item;

public class ItemCollectionSelectedEventArgs : EventArgs
{
	public ItemSO Item { get; set; }

	public CollectionItemUI CollectionItemUI { get; set; }

	public ItemCollectionSelectedEventArgs(ItemSO item, CollectionItemUI collectionItemUI)
	{
		Item = item;
		CollectionItemUI = collectionItemUI;
	}
}
