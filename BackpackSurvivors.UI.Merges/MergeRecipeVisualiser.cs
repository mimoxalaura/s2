using BackpackSurvivors.Game.Core;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Merges;

internal class MergeRecipeVisualiser : MonoBehaviour
{
	[SerializeField]
	private Transform _recipeContainer;

	[SerializeField]
	private MergeRecipeVisualItem _mergeRecipeVisualItemPrefab;

	private void Start()
	{
		foreach (MergableSO availableMergable in SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableMergables)
		{
			Object.Instantiate(_mergeRecipeVisualItemPrefab, _recipeContainer).Init(availableMergable, unlocked: true);
		}
	}
}
