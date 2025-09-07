using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.Merging;

internal class MergeToastController : SingletonController<MergeToastController>
{
	[Header("Toast")]
	[SerializeField]
	private MergeToastNotification _mergeToastNotificationPrefab;

	[SerializeField]
	private Transform _mergeToastNotificationContainer;

	internal void ShowCompleteMergeToasts(BaseItemInstance lastDropped)
	{
		BaseDraggable[] array = Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		foreach (BaseDraggable baseDraggable in array)
		{
			if (baseDraggable.Owner != Enums.Backpack.DraggableOwner.Shop && baseDraggable.StoredInGridType != Enums.Backpack.GridType.Storage && (baseDraggable.BaseItemInstance.CurrentMergeRecipeSet != null || baseDraggable.BaseItemInstance.CurrentMergeRecipeSet.RecipeIsComplete))
			{
				ShowCompleteMergeToast(baseDraggable.BaseItemInstance);
			}
		}
	}

	internal void ShowIncompleteMergeToasts()
	{
		BaseDraggable[] array = Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		foreach (BaseDraggable baseDraggable in array)
		{
			if (baseDraggable.Owner != Enums.Backpack.DraggableOwner.Shop && baseDraggable.StoredInGridType != Enums.Backpack.GridType.Storage && baseDraggable.BaseItemInstance.CurrentMergeRecipeSet == null)
			{
				ShowIncompleteMergeToast(baseDraggable.BaseItemInstance);
			}
		}
	}

	internal void ShowCombiningLockedToast(BaseDraggable currentlyHoveredDraggable)
	{
		if (!currentlyHoveredDraggable.BaseItemInstance.MergingAllowed)
		{
			Vector2 locationToShow = GetLocationToShow(currentlyHoveredDraggable.BaseItemInstance);
			MergeToastNotification mergeBlockedToast = GetMergeBlockedToast(locationToShow);
			mergeBlockedToast.Display();
			currentlyHoveredDraggable.BaseItemInstance.SetActiveToast(mergeBlockedToast);
		}
	}

	internal void ShowCompleteMergeToast(BaseItemInstance baseItemInstance)
	{
		Vector2 locationToShow = GetLocationToShow(baseItemInstance);
		if (!(baseItemInstance.CurrentMergeRecipeSet.PrimaryBaseItemInstance.Guid == baseItemInstance.Guid))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (MergeableIngredient otherIngredientInRecipe in baseItemInstance.CurrentMergeRecipeSet.Recipe.Input.Where((MergeableIngredient x) => x.ItemName != baseItemInstance.BaseItemSO.Name))
		{
			num2 += otherIngredientInRecipe.Amount;
			num += baseItemInstance.CurrentMergeRecipeSet.AvailableItemsForMerge.Count((BaseItemInstance x) => x.BaseItemSO.Name == otherIngredientInRecipe.ItemName);
		}
		MergeToastNotification mergeResultToast = GetMergeResultToast(locationToShow, baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, num2, num);
		mergeResultToast.Display();
		baseItemInstance.SetActiveToast(mergeResultToast);
	}

	internal void ShowIncompleteMergeToast(BaseItemInstance baseItemInstance)
	{
		Vector2 locationToShow = GetLocationToShow(baseItemInstance);
		if (!(baseItemInstance.CurrentMergeRecipeSet.PrimaryBaseItemInstance.Guid == baseItemInstance.Guid))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (MergeableIngredient otherIngredientInRecipe in baseItemInstance.CurrentMergeRecipeSet.Recipe.Input.Where((MergeableIngredient x) => x.ItemName != baseItemInstance.BaseItemSO.Name))
		{
			num2 += otherIngredientInRecipe.Amount;
			num += baseItemInstance.CurrentMergeRecipeSet.AvailableItemsForMerge.Count((BaseItemInstance x) => x.BaseItemSO.Name == otherIngredientInRecipe.ItemName);
		}
		MergeToastNotification mergeResultToast = GetMergeResultToast(locationToShow, baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, num2, num);
		mergeResultToast.Display();
		baseItemInstance.SetActiveToast(mergeResultToast);
	}

	internal void ShowNoPlaceForMergeResult(BaseItemInstance baseItemInstance)
	{
		Vector2 locationToShow = GetLocationToShow(baseItemInstance);
		GetNoPlaceForMergeResultToast(locationToShow).Display();
	}

	private Vector2 GetLocationToShow(BaseItemInstance baseItemInstance)
	{
		Vector2 centerOfItemInGrid = SingletonController<BackpackController>.Instance.GetCenterOfItemInGrid(baseItemInstance);
		float num = 1f / SingletonController<BackpackController>.Instance.Canvas.transform.localScale.x;
		return new Vector2(centerOfItemInGrid.x * num, centerOfItemInGrid.y * num);
	}

	private MergeToastNotification GetMergeResultToast(Vector2 positionToShow, BaseItemSO mergeResultItemSO, int amountNeeded, int amountCurrent)
	{
		MergeToastNotification mergeToastNotification = Object.Instantiate(_mergeToastNotificationPrefab, _mergeToastNotificationContainer);
		mergeToastNotification.UpdatePosition(positionToShow);
		mergeToastNotification.UpdateTextByMergeResult(mergeResultItemSO, amountNeeded, amountCurrent);
		return mergeToastNotification;
	}

	private MergeToastNotification GetMergeBlockedToast(Vector2 positionToShow)
	{
		MergeToastNotification mergeToastNotification = Object.Instantiate(_mergeToastNotificationPrefab, _mergeToastNotificationContainer);
		mergeToastNotification.UpdatePosition(positionToShow);
		mergeToastNotification.UpdateMergingBlockedText();
		return mergeToastNotification;
	}

	private MergeToastNotification GetNoPlaceForMergeResultToast(Vector2 positionToShow)
	{
		MergeToastNotification mergeToastNotification = Object.Instantiate(_mergeToastNotificationPrefab, _mergeToastNotificationContainer);
		mergeToastNotification.UpdatePosition(positionToShow);
		mergeToastNotification.UpdateNoPlaceForMergeResultText();
		return mergeToastNotification;
	}

	[Command("merge.ShowToast", Platform.AllPlatforms, MonoTargetType.Single)]
	public void DEBUG_TOAST(Enums.PlaceableType placeableType, int baseItemSOID)
	{
		Vector2 positionToShow = new Vector2(Screen.width / 2, Screen.height / 2);
		switch (placeableType)
		{
		case Enums.PlaceableType.Weapon:
			GetMergeResultToast(positionToShow, GameDatabaseHelper.GetWeaponById(baseItemSOID), 1, 2).Display();
			break;
		case Enums.PlaceableType.Bag:
			Debug.LogWarning("Bag merging is not implemented");
			break;
		case Enums.PlaceableType.Item:
			GetMergeResultToast(positionToShow, GameDatabaseHelper.GetItemById(baseItemSOID), 1, 2).Display();
			break;
		}
	}
}
