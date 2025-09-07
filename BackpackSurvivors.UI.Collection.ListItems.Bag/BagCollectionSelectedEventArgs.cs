using System;
using BackpackSurvivors.ScriptableObjects.Items;

namespace BackpackSurvivors.UI.Collection.ListItems.Bag;

public class BagCollectionSelectedEventArgs : EventArgs
{
	public BagSO Bag { get; }

	public CollectionBagUI CollectionBagUI { get; set; }

	public BagCollectionSelectedEventArgs(BagSO bag, CollectionBagUI collectionBagUI)
	{
		Bag = bag;
		CollectionBagUI = collectionBagUI;
	}
}
