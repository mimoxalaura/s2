using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack.Interfaces;
using BackpackSurvivors.Game.Combat.Weapons.ExternalStats;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.BackpackVFX;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Shop;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;

namespace BackpackSurvivors.Game.Backpack;

internal class DraggableWeapon : BaseDraggable
{
	[SerializeField]
	private WeaponTooltipTrigger _weaponTooltipTrigger;

	[SerializeField]
	private Transform _highlightOffsetParentTransform;

	[SerializeField]
	private BackpackDropItemVFX _backpackDropItemVFX;

	[SerializeField]
	private ImageAnimationTimeScaleZero _imageAnimation;

	private List<int> _originalStarSlotids = new List<int>();

	internal List<int> CurrentStarSlotids = new List<int>();

	private BackpackVisualGrid _previousVisualGrid;

	internal WeaponInstance WeaponInstance { get; private set; }

	private protected override void Awake()
	{
		base.Awake();
		RegisterEvents();
	}

	internal void Init(WeaponSO weaponSO, ShopOfferSlot parentSlot = null, bool applySize = true)
	{
		WeaponSO weaponSO2 = GetWeaponSO(weaponSO);
		Init(weaponSO2, Enums.Backpack.DraggableType.Weapon, parentSlot, applySize);
		WeaponInstance = new WeaponInstance(weaponSO2);
		SetBaseItemInstance(WeaponInstance);
		SetImage(weaponSO2.BackpackImage);
		_weaponTooltipTrigger.SetWeaponContent(WeaponInstance, base.Owner);
		if (weaponSO.BackpackAnimatedImages.Any())
		{
			_imageAnimation.SetFrames(weaponSO.BackpackAnimatedImages);
		}
	}

	internal override void OverridePriceInTooltip(int discountedPrice)
	{
		base.OverridePriceInTooltip(discountedPrice);
		_weaponTooltipTrigger.SetDiscountedPrice(discountedPrice);
	}

	internal void SetStartStarSlotIds(List<int> startStarSlotIds)
	{
		CurrentStarSlotids = startStarSlotIds;
	}

	private WeaponSO GetWeaponSO(WeaponSO originalWeaponSO)
	{
		if (ExternalWeaponStats.ExternalWeaponStatsAvailable())
		{
			return ExternalWeaponStats.GetWeaponSO(originalWeaponSO.Id, originalWeaponSO);
		}
		return originalWeaponSO;
	}

	private void RegisterEvents()
	{
		OnRotationChanged = (EventHandler)Delegate.Combine(OnRotationChanged, new EventHandler(Base_OnRotationChanged));
		base.OnOwnerChanged += DraggableWeapon_OnOwnerChanged;
	}

	private void DraggableWeapon_OnOwnerChanged(object sender, EventArgs e)
	{
		_weaponTooltipTrigger.ChangeOwner(base.Owner);
	}

	private void Base_OnRotationChanged(object sender, EventArgs e)
	{
		CalculatePlaceable();
		Drag();
	}

	private void HighlightInvalidSlots(BackpackVisualGrid visualGrid, List<int> invalidSlotIds, List<int> itemSlotIds)
	{
		foreach (int itemSlotId in itemSlotIds)
		{
			Color color = ((!invalidSlotIds.Contains(itemSlotId)) ? Constants.Colors.ValidBackpackPlacementColor : Constants.Colors.InvalidBackpackPlacementColor);
			visualGrid.ShowCellStatusColor(itemSlotId, color);
		}
	}

	private void HighlightValidSlots(BackpackVisualGrid visualGrid, List<int> itemSlotIds)
	{
		foreach (int itemSlotId in itemSlotIds)
		{
			visualGrid.ShowCellStatusColor(itemSlotId, Constants.Colors.ValidBackpackPlacementColor);
		}
	}

	private void HighlightStarSlots(BackpackVisualGrid visualGrid, List<int> starredSlots)
	{
		foreach (int starredSlot in starredSlots)
		{
			visualGrid.SetStarImageVisibility(starredSlot, visibile: true, showStarredFilled: false, base.BaseItemSO.StarringEffectIsPositive);
		}
	}

	private void HighlightCurrentCellSlots(BackpackVisualGrid visualGrid, List<int> currentCellSlots)
	{
		foreach (int currentCellSlot in currentCellSlots)
		{
			visualGrid.SetCellOutlineVisibility(currentCellSlot, visible: true);
		}
	}

	internal override void BeginDrag()
	{
		RemoveWeaponFromGrid();
		base.BeginDrag();
	}

	internal override void Drag()
	{
		SingletonController<BackpackController>.Instance.ClearAllStarImages();
		SingletonController<BackpackController>.Instance.ToggleStorageChest(SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().GridType == Enums.Backpack.GridType.Storage, playAudio: false);
		BackpackVisualGrid hoveredVisualGrid = SingletonController<BackpackController>.Instance.GetHoveredVisualGrid();
		HoveredSlotInfo hoveredSlotInfo = SingletonController<BackpackController>.Instance.HoveredSlotProvider.GetHoveredSlotInfo();
		if (hoveredSlotInfo.Equals(HoveredSlotInfo.None))
		{
			SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
			hoveredVisualGrid.HideAllCellStatusColors();
			return;
		}
		if (hoveredSlotInfo.HoveredCellGridType == Enums.Backpack.GridType.Storage)
		{
			SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
		}
		List<int> slotIdsToPlaceItemIn = SingletonController<BackpackController>.Instance.GetHoveredGridSlotCalculator().GetSlotIdsToPlaceItemIn(base._placeableInfo, hoveredSlotInfo);
		SingletonController<BackpackController>.Instance.HighlightBagSlotsWherePlaceableWouldBePlaced(slotIdsToPlaceItemIn);
		if (CanPlaceEntireItemInCells(slotIdsToPlaceItemIn))
		{
			int topLeftSlotId = slotIdsToPlaceItemIn.Min((int s) => s);
			UpdateCellHighlights(hoveredVisualGrid, slotIdsToPlaceItemIn, CurrentStarSlotids = SingletonController<BackpackController>.Instance.GetHoveredGridSlotCalculator().GetSlotIdsToPlaceStarsIn(base._placeableInfo, topLeftSlotId));
		}
	}

	private List<int> UpdateCellHighlights(BackpackVisualGrid visualGrid, List<int> itemSlotIds, List<int> starredSlotIds)
	{
		List<int> invalidCellIds = new List<int>();
		visualGrid.HideAllCellStatusColors();
		visualGrid.HideAllStarImages();
		if (SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().CanPlaceWeapon(WeaponInstance, itemSlotIds, out invalidCellIds))
		{
			HighlightValidSlots(visualGrid, itemSlotIds);
		}
		else
		{
			HighlightInvalidSlots(visualGrid, invalidCellIds, itemSlotIds);
		}
		HighlightStarSlots(visualGrid, starredSlotIds);
		return invalidCellIds;
	}

	internal override bool Drop()
	{
		HoveredSlotInfo hoveredSlotInfo = SingletonController<BackpackController>.Instance.HoveredSlotProvider.GetHoveredSlotInfo();
		if (hoveredSlotInfo.Equals(HoveredSlotInfo.None))
		{
			RevertDrop();
			return false;
		}
		IBackpackStorage hoveredBackpackStorage = SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage();
		bool addingWeapon = (base.Owner == Enums.Backpack.DraggableOwner.Shop || base.StoredInGridType == Enums.Backpack.GridType.Storage) && hoveredBackpackStorage.GridType == Enums.Backpack.GridType.Backpack;
		if (SingletonController<StatController>.Instance.WeaponCapacityReached(addingWeapon))
		{
			SingletonController<StatController>.Instance.ShowCapacityReachedPopup();
			RevertDrop();
			return false;
		}
		SlotCalculator hoveredGridSlotCalculator = SingletonController<BackpackController>.Instance.GetHoveredGridSlotCalculator();
		List<int> slotIdsToPlaceItemIn = hoveredGridSlotCalculator.GetSlotIdsToPlaceItemIn(base._placeableInfo, hoveredSlotInfo);
		if (!CanPlaceEntireItemInCells(slotIdsToPlaceItemIn))
		{
			RevertDrop();
			return false;
		}
		List<int> invalidCellIds = new List<int>();
		if (hoveredBackpackStorage.CanPlaceWeapon(WeaponInstance, slotIdsToPlaceItemIn, out invalidCellIds))
		{
			if (!TryBuyIfNeeded())
			{
				RevertDrop();
				return false;
			}
			int topLeftSlotId = slotIdsToPlaceItemIn.Min((int s) => s);
			List<int> slotIdsToPlaceStarsIn = hoveredGridSlotCalculator.GetSlotIdsToPlaceStarsIn(base._placeableInfo, topLeftSlotId);
			if (!hoveredBackpackStorage.PlaceWeapon(WeaponInstance, slotIdsToPlaceItemIn, slotIdsToPlaceStarsIn))
			{
				RevertDrop();
				return false;
			}
			SingletonController<BackpackController>.Instance.MoveToStorage(hoveredBackpackStorage.ItemsToMoveToStorage, hoveredBackpackStorage.WeaponsToMoveToStorage, new List<BagInstance>());
			SingletonController<BackpackController>.Instance.ClearMoveToStorage();
			base.StartItemSlotids = slotIdsToPlaceItemIn;
			_originalStarSlotids = slotIdsToPlaceStarsIn;
			SetStoreInGridType(hoveredBackpackStorage.GridType);
			CenterDraggableOnItemSlots(slotIdsToPlaceItemIn);
			ResetStatsIfDroppedInStorage(hoveredBackpackStorage);
			base.transform.SetParent(SingletonController<BackpackController>.Instance.BoughtItemsAndWeaponsParentTransform);
			WeaponInstance.UpdateDroppedInBackpackTime();
			ResetScaleAndZIndex();
			if (hoveredBackpackStorage.GridType == Enums.Backpack.GridType.Backpack)
			{
				UnityEngine.Object.Instantiate(_backpackDropItemVFX, base.transform).Init(WeaponInstance.BaseWeaponSO, base.VFXImage);
			}
			return true;
		}
		RevertDrop();
		return false;
	}

	private void ResetStatsIfDroppedInStorage(IBackpackStorage backpackStorage)
	{
		if (backpackStorage is OutOfBackpackStorage)
		{
			WeaponInstance.ResetStats();
		}
	}

	internal override void EndDrag(bool dropWasSuccess)
	{
		BackpackVisualGrid hoveredVisualGrid = SingletonController<BackpackController>.Instance.GetHoveredVisualGrid();
		hoveredVisualGrid.HideAllCellStatusColors();
		hoveredVisualGrid.HideAllStarImages();
		SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
		base.EndDrag(dropWasSuccess);
	}

	internal override void MoveToStorage()
	{
		if (base.StoredInGridType != Enums.Backpack.GridType.Storage)
		{
			SingletonController<BackpackController>.Instance.MoveToStorage(new List<ItemInstance>(), new List<WeaponInstance> { WeaponInstance }, new List<BagInstance>());
		}
	}

	internal override void RevertDrop()
	{
		RotateBackToPreviousRotation();
		PlaceWeaponBackIntoGrid();
		ReturnToStartOfDrag();
		CurrentStarSlotids = _originalStarSlotids;
		SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
	}

	private void PlaceWeaponBackIntoGrid()
	{
		if (base.StartItemSlotids.Any())
		{
			SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).PlaceWeapon(WeaponInstance, base.StartItemSlotids, _originalStarSlotids);
		}
	}

	private void RemoveWeaponFromGrid()
	{
		SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).RemoveWeapon(WeaponInstance);
	}

	internal override void PointerEnter()
	{
		base.PointerEnter();
		if (CanInteract)
		{
			SingletonController<MergeController>.Instance.DrawLinesToPotentialMergables(this);
			if (base.Owner != Enums.Backpack.DraggableOwner.Shop)
			{
				SingletonController<BackpackController>.Instance.DrawLinesToAffectedWeaponsAndItems(this, WeaponInstance.BaseItemSO.StarringEffectIsPositive);
				SingletonController<BackpackController>.Instance.HighlightAffectedWeaponsAndItems(this, animate: false, WeaponInstance.BaseItemSO.StarringEffectIsPositive);
				BackpackVisualGrid visualGridByStorageType = SingletonController<BackpackController>.Instance.GetVisualGridByStorageType(base.StoredInGridType);
				HighlightStarSlots(visualGridByStorageType, CurrentStarSlotids);
				HighlightCurrentCellSlots(visualGridByStorageType, base.StartItemSlotids);
				_previousVisualGrid = visualGridByStorageType;
			}
		}
	}

	internal override void PointerExit()
	{
		base.PointerExit();
		if (CanInteract)
		{
			SingletonController<StarLineController>.Instance.ClearStarredLines();
			SingletonController<StarLineController>.Instance.ClearHighlights();
			SingletonController<MergeController>.Instance.ClearPotentialLines();
			if (_previousVisualGrid != null)
			{
				_previousVisualGrid.HideAllCellOutlines();
				_previousVisualGrid.HideAllStarImages();
			}
		}
	}
}
