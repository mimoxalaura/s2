using System;
using BackpackSurvivors.ScriptableObjects.Classes;

namespace BackpackSurvivors.UI.Collection.ListItems.Enemy;

public class EnemyCollectionSelectedEventArgs : EventArgs
{
	public EnemySO Enemy { get; set; }

	public CollectionEnemyUI CollectionEnemyUI { get; set; }

	public EnemyCollectionSelectedEventArgs(EnemySO enemy, CollectionEnemyUI collectionEnemyUI)
	{
		Enemy = enemy;
		CollectionEnemyUI = collectionEnemyUI;
	}
}
