using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items.Merging;

internal class Mergable
{
	private BaseItemInstance _baseItemInstance;

	private List<int> _originalCellIds = new List<int>();

	private List<int> _cellIdsToPlaceOn = new List<int>();

	private Enums.Backpack.ItemRotation _rotation;

	public Mergable(BaseItemInstance baseItemInstance)
	{
		_baseItemInstance = baseItemInstance;
	}

	internal Transform AnimateMerge(float animationDuration)
	{
		List<BaseItemInstance> list = new List<BaseItemInstance>();
		list.AddRange(_baseItemInstance.CurrentMergeRecipeSet.AvailableItemsForMerge);
		Color targetColor = new Color(241f, 0f, 157f);
		float num = animationDuration / 3f;
		_baseItemInstance.BaseDraggable.Image.color = targetColor;
		_baseItemInstance.BaseDraggable.HideVfxImage();
		foreach (BaseItemInstance otherItem in list)
		{
			otherItem.BaseDraggable.Image.color = targetColor;
			otherItem.BaseDraggable.HideVfxImage();
			LeanTween.moveLocal(otherItem.BaseDraggable.gameObject, _baseItemInstance.BaseDraggable.transform.localPosition, animationDuration).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.value(otherItem.BaseDraggable.gameObject, 1f, 0.5f, num * 3f).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true)
				.setOnUpdate((Action<float>)delegate
				{
					otherItem.BaseDraggable.Image.color = targetColor;
				});
		}
		LeanTween.value(_baseItemInstance.BaseDraggable.gameObject, 1f, 0.5f, num * 3f).setDelay(num).setIgnoreTimeScale(useUnScaledTime: true)
			.setOnUpdate((Action<float>)delegate
			{
				_baseItemInstance.BaseDraggable.Image.color = targetColor;
			});
		return _baseItemInstance.BaseDraggable.transform;
	}

	internal bool CanPlaceMergeResult(bool showCannotPlaceMergeResultToast = false)
	{
		FillOriginalCellids();
		if (CanPlaceInBackpack())
		{
			return true;
		}
		if (CanPlaceInStorage())
		{
			return true;
		}
		if (showCannotPlaceMergeResultToast)
		{
			SingletonController<MergeToastController>.Instance.ShowNoPlaceForMergeResult(_baseItemInstance);
		}
		return false;
	}

	private bool CanPlaceInStorage()
	{
		return _baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem.ItemType switch
		{
			Enums.PlaceableType.Weapon => SingletonController<BackpackController>.Instance.CanPlaceWeaponInStorage((WeaponSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem), 
			Enums.PlaceableType.Bag => SingletonController<BackpackController>.Instance.CanPlaceBagInStorage((BagSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem), 
			Enums.PlaceableType.Item => SingletonController<BackpackController>.Instance.CanPlaceItemInStorage((ItemSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem), 
			_ => false, 
		};
	}

	internal void ExecuteMerge()
	{
		FillOriginalCellids();
		if (CanPlaceInBackpack())
		{
			RemoveSourcePlaceables();
			AddMergeResultToBackpack();
			SingletonController<BackpackController>.Instance.ClearMoveToStorage();
		}
		else if (TryPlaceMergeResultInStorage())
		{
			RemoveSourcePlaceables();
		}
	}

	private void AddMergeResultToBackpack()
	{
		switch (_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem.ItemType)
		{
		case Enums.PlaceableType.Weapon:
			SingletonController<BackpackController>.Instance.AddWeaponToBackpack((WeaponSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, _cellIdsToPlaceOn, _rotation);
			break;
		case Enums.PlaceableType.Item:
			SingletonController<BackpackController>.Instance.AddItemToBackpack((ItemSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, _cellIdsToPlaceOn, _rotation);
			break;
		}
	}

	private bool CanPlaceInBackpack()
	{
		return SingletonController<BackpackController>.Instance.CanPlaceMergable(_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, _baseItemInstance.CurrentMergeRecipeSet.AvailableItemsForMerge, _originalCellIds, out _cellIdsToPlaceOn, out _rotation);
	}

	private void FillOriginalCellids()
	{
		_originalCellIds.AddRange(SingletonController<BackpackController>.Instance.BackpackStorage.GetCellsPlaceableIsIn(_baseItemInstance.CurrentMergeRecipeSet.PrimaryBaseItemInstance));
		foreach (BaseItemInstance item in _baseItemInstance.CurrentMergeRecipeSet.AvailableItemsForMergeExcludingPrimaryItem)
		{
			_originalCellIds.AddRange(SingletonController<BackpackController>.Instance.BackpackStorage.GetCellsPlaceableIsIn(item));
		}
	}

	private void RemoveSourcePlaceables()
	{
		foreach (BaseItemInstance item in _baseItemInstance.CurrentMergeRecipeSet.AvailableItemsForMerge)
		{
			SingletonController<BackpackController>.Instance.BackpackStorage.RemoveBaseItemInstance(item);
			DragController.DestroyDraggable(item);
		}
	}

	private bool TryPlaceMergeResultInStorage()
	{
		return _baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem.ItemType switch
		{
			Enums.PlaceableType.Weapon => SingletonController<BackpackController>.Instance.AddWeaponToStorage((WeaponSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, showVfx: true, fromMerge: true), 
			Enums.PlaceableType.Item => SingletonController<BackpackController>.Instance.AddItemToStorage((ItemSO)_baseItemInstance.CurrentMergeRecipeSet.Recipe.Output.BaseItem, showVfx: true, fromMerge: true), 
			_ => false, 
		};
	}
}
