using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack.Interfaces;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.ExternalStats;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Items.RuneEffects;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.BackpackVFX;
using BackpackSurvivors.UI.Shop;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class DraggableItem : BaseDraggable
{
	[SerializeField]
	private ItemTooltipTrigger _itemTooltipTrigger;

	[SerializeField]
	private Transform _highlightOffsetParentTransform;

	[SerializeField]
	private BackpackDropItemVFX _backpackDropItemVFX;

	private List<int> _originalStarSlotids = new List<int>();

	internal List<int> CurrentStarSlotids = new List<int>();

	private BackpackVisualGrid _previousVisualGrid;

	internal ItemInstance ItemInstance { get; private set; }

	private protected override void Awake()
	{
		base.Awake();
		RegisterEvents();
	}

	internal void Init(ItemSO itemSO, ShopOfferSlot parentSlot = null, bool applySize = true)
	{
		ItemSO itemSO2 = GetItemSO(itemSO);
		Init(itemSO2, Enums.Backpack.DraggableType.Item, parentSlot, applySize);
		ItemInstance = new ItemInstance(itemSO2);
		SetBaseItemInstance(ItemInstance);
		SetImage(itemSO2.BackpackImage);
		if (itemSO.HasSpecialEffect)
		{
			RuneSpecialEffect runeSpecialEffect = UnityEngine.Object.Instantiate(itemSO.RuneSpecialEffect, base.transform);
			runeSpecialEffect.Init(this);
			ItemInstance.RuneSpecialEffect = runeSpecialEffect;
		}
		_itemTooltipTrigger.SetItemContent(ItemInstance, base.Owner);
	}

	internal void SetStartStarSlotIds(List<int> startStarSlotIds)
	{
		CurrentStarSlotids = startStarSlotIds;
	}

	internal override void OverridePriceInTooltip(int discountedPrice)
	{
		base.OverridePriceInTooltip(discountedPrice);
		_itemTooltipTrigger.SetDiscountedPrice(discountedPrice);
	}

	private ItemSO GetItemSO(ItemSO originalItemSO)
	{
		if (ExternalItemStats.ExternalItemStatsAvailable())
		{
			return ExternalItemStats.GetItemSO(originalItemSO.Id, originalItemSO);
		}
		return originalItemSO;
	}

	private void RegisterEvents()
	{
		OnRotationChanged = (EventHandler)Delegate.Combine(OnRotationChanged, new EventHandler(Base_OnRotationChanged));
		base.OnOwnerChanged += DraggableItem_OnOwnerChanged;
	}

	private void DraggableItem_OnOwnerChanged(object sender, EventArgs e)
	{
		_itemTooltipTrigger.ChangeOwner(base.Owner);
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
			WeaponInstance weaponFromSlot = SingletonController<BackpackController>.Instance.BackpackStorage.GetWeaponFromSlot(starredSlot);
			bool showStarredFilled = CanAffectWeapon(weaponFromSlot);
			visualGrid.SetStarImageVisibility(starredSlot, visibile: true, showStarredFilled, base.BaseItemSO.StarringEffectIsPositive);
		}
	}

	private void HighlightCurrentCellSlots(BackpackVisualGrid visualGrid, List<int> currentCellSlots)
	{
		foreach (int currentCellSlot in currentCellSlots)
		{
			visualGrid.SetCellOutlineVisibility(currentCellSlot, visible: true);
		}
	}

	private bool CanAffectWeapon(WeaponInstance weaponInSlot)
	{
		if (weaponInSlot == null)
		{
			return false;
		}
		return ItemInstance.CanAffectWeaponInstance(weaponInSlot);
	}

	internal override void BeginDrag()
	{
		UnregisterConditions();
		RemoveItemFromGrid();
		base.BeginDrag();
	}

	private void UnregisterConditions()
	{
		if (base.StoredInGridType == Enums.Backpack.GridType.Backpack && base.Owner != Enums.Backpack.DraggableOwner.Shop)
		{
			ConditionSO[] array = ItemInstance.ItemSO.ConditionalStats?.Conditions;
			if (array != null)
			{
				SingletonController<RecalculateStatsTriggerController>.Instance.UnregisterConditions(array);
			}
		}
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
		SlotCalculator hoveredGridSlotCalculator = SingletonController<BackpackController>.Instance.GetHoveredGridSlotCalculator();
		List<int> slotIdsToPlaceItemIn = hoveredGridSlotCalculator.GetSlotIdsToPlaceItemIn(base._placeableInfo, hoveredSlotInfo);
		SingletonController<BackpackController>.Instance.HighlightBagSlotsWherePlaceableWouldBePlaced(slotIdsToPlaceItemIn);
		if (CanPlaceEntireItemInCells(slotIdsToPlaceItemIn))
		{
			int topLeftSlotId = slotIdsToPlaceItemIn.Min((int s) => s);
			UpdateCellHighlights(hoveredVisualGrid, slotIdsToPlaceItemIn, CurrentStarSlotids = hoveredGridSlotCalculator.GetSlotIdsToPlaceStarsIn(base._placeableInfo, topLeftSlotId));
		}
	}

	private List<int> UpdateCellHighlights(BackpackVisualGrid visualGrid, List<int> itemSlotIds, List<int> starredSlotIds)
	{
		List<int> invalidCellIds = new List<int>();
		visualGrid.HideAllCellStatusColors();
		visualGrid.HideAllStarImages();
		if (SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().CanPlaceItem(ItemInstance, itemSlotIds, out invalidCellIds))
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

	internal override void Dissolve()
	{
		base.Dissolve();
		_itemTooltipTrigger.enabled = false;
		float num = 2.5f;
		StartCoroutine(DissolveAsync(num));
		Image component = GetComponent<Image>();
		component.material = UnityEngine.Object.Instantiate(component.material);
		Material material = component.materialForRendering;
		LeanTween.value(base.gameObject, delegate(float val)
		{
			material.SetFloat("_FullGlowDissolveFade", val);
		}, 1f, 0f, num).setIgnoreTimeScale(useUnScaledTime: true);
	}

	internal IEnumerator DissolveAsync(float duration)
	{
		yield return new WaitForSecondsRealtime(duration);
		SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).RemoveItem(ItemInstance);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	internal override bool Drop()
	{
		IBackpackStorage hoveredBackpackStorage = SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage();
		bool addingToBackpack = (base.Owner == Enums.Backpack.DraggableOwner.Shop || base.StoredInGridType == Enums.Backpack.GridType.Storage) && hoveredBackpackStorage.GridType == Enums.Backpack.GridType.Backpack;
		bool removingFromBackpack = base.Owner == Enums.Backpack.DraggableOwner.Player && base.StoredInGridType == Enums.Backpack.GridType.Backpack && hoveredBackpackStorage.GridType == Enums.Backpack.GridType.Storage;
		if (DropWouldViolateWeaponCapacity(hoveredBackpackStorage, addingToBackpack, removingFromBackpack))
		{
			SingletonController<StatController>.Instance.ShowCapacityReachedPopup();
			RevertDrop();
			return false;
		}
		HoveredSlotInfo hoveredSlotInfo = SingletonController<BackpackController>.Instance.HoveredSlotProvider.GetHoveredSlotInfo();
		if (hoveredSlotInfo.Equals(HoveredSlotInfo.None))
		{
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
		if (hoveredBackpackStorage.CanPlaceItem(ItemInstance, slotIdsToPlaceItemIn, out invalidCellIds))
		{
			if (!TryBuyIfNeeded())
			{
				RevertDrop();
				return false;
			}
			int topLeftSlotId = slotIdsToPlaceItemIn.Min((int s) => s);
			List<int> slotIdsToPlaceStarsIn = hoveredGridSlotCalculator.GetSlotIdsToPlaceStarsIn(base._placeableInfo, topLeftSlotId);
			if (!hoveredBackpackStorage.PlaceItem(ItemInstance, slotIdsToPlaceItemIn, slotIdsToPlaceStarsIn))
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
			base.transform.SetParent(SingletonController<BackpackController>.Instance.BoughtItemsAndWeaponsParentTransform);
			ItemInstance.UpdateDroppedInBackpackTime();
			ResetScaleAndZIndex();
			if (hoveredBackpackStorage.GridType == Enums.Backpack.GridType.Backpack && ItemInstance.ItemSO.ConditionalStats != null)
			{
				SingletonController<RecalculateStatsTriggerController>.Instance.RegisterConditions(ItemInstance.ItemSO.ConditionalStats.Conditions);
			}
			if (hoveredBackpackStorage.GridType == Enums.Backpack.GridType.Backpack)
			{
				UnityEngine.Object.Instantiate(_backpackDropItemVFX, base.transform).Init(ItemInstance.ItemSO, base.VFXImage);
			}
			return true;
		}
		RevertDrop();
		return false;
	}

	private bool DropWouldViolateWeaponCapacity(IBackpackStorage backpackStorage, bool addingToBackpack, bool removingFromBackpack)
	{
		float valueOrDefault = ItemInstance.CalculatedStats.GetValueOrDefault(Enums.ItemStatType.WeaponCapacity, 0f);
		if (valueOrDefault == 0f)
		{
			return false;
		}
		float valueOrDefault2 = SingletonController<GameController>.Instance.Player.CalculatedStats.GetValueOrDefault(Enums.ItemStatType.WeaponCapacity, 0f);
		int placeableTypeCount = SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(Enums.PlaceableType.Weapon);
		if (valueOrDefault > 0f && removingFromBackpack)
		{
			return valueOrDefault2 - valueOrDefault < (float)placeableTypeCount;
		}
		if (valueOrDefault < 0f && addingToBackpack)
		{
			return valueOrDefault2 + valueOrDefault < (float)placeableTypeCount;
		}
		return false;
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
			IBackpackStorage hoveredBackpackStorage = SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage();
			if (!DropWouldViolateWeaponCapacity(hoveredBackpackStorage, addingToBackpack: false, removingFromBackpack: true))
			{
				SingletonController<BackpackController>.Instance.MoveToStorage(new List<ItemInstance> { ItemInstance }, new List<WeaponInstance>(), new List<BagInstance>());
			}
		}
	}

	internal override void RevertDrop()
	{
		RotateBackToPreviousRotation();
		PlaceItemBackIntoGrid();
		ReturnToStartOfDrag();
		CurrentStarSlotids = _originalStarSlotids;
		SingletonController<BackpackController>.Instance.ResetBagSlotHighlights();
	}

	private void PlaceItemBackIntoGrid()
	{
		if (base.StartItemSlotids.Any())
		{
			SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).PlaceItem(ItemInstance, base.StartItemSlotids, _originalStarSlotids);
		}
	}

	private void RemoveItemFromGrid()
	{
		SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).RemoveItem(ItemInstance);
	}

	internal override void PointerEnter()
	{
		base.PointerEnter();
		if (CanInteract)
		{
			SingletonController<MergeController>.Instance.DrawLinesToPotentialMergables(this);
			if (base.Owner != Enums.Backpack.DraggableOwner.Shop)
			{
				SingletonController<BackpackController>.Instance.DrawLinesToAffectedWeaponsAndItems(this, ItemInstance.ItemSO.StarringEffectIsPositive);
				SingletonController<BackpackController>.Instance.HighlightAffectedWeaponsAndItems(this, animate: false, ItemInstance.ItemSO.StarringEffectIsPositive);
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
