using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.Game.Backpack.Highlighting;
using BackpackSurvivors.Game.Backpack.Interfaces;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shop;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class DraggableBag : BaseDraggable
{
	[SerializeField]
	private BagTooltipTrigger _bagTooltipTrigger;

	[SerializeField]
	private GameObject _highlightCellsParent;

	[SerializeField]
	private BagCellHighlightImage _bagCellHighlightImagePrefab;

	[SerializeField]
	private Image _bagImage;

	private bool _isDragging;

	private List<BagCellHighlightImage> _cellHighlightImages = new List<BagCellHighlightImage>();

	internal BagInstance BagInstance { get; private set; }

	private protected override void Awake()
	{
		base.Awake();
		RegisterBaseEvents();
	}

	internal void Init(BagSO bagSO, ShopOfferSlot parentSlot = null, bool applySize = true)
	{
		Init(bagSO, Enums.Backpack.DraggableType.Bag, parentSlot, applySize);
		BagInstance = new BagInstance(bagSO);
		SetBaseItemInstance(BagInstance);
		SetImage(bagSO.BackpackImageWithoutSlots);
		_bagTooltipTrigger.SetBag(bagSO, active: true, base.Owner);
		SetImageAlpha(1f);
	}

	internal override void OverridePriceInTooltip(int discountedPrice)
	{
		base.OverridePriceInTooltip(discountedPrice);
		_bagTooltipTrigger.SetDiscountedPrice(discountedPrice);
	}

	internal void HighlightBagCellsWherePlaceableWouldBePlaced(List<int> placeableSlotIds, List<int> emptyBagSlotIds)
	{
		foreach (BagCellHighlightImage cellHighlightImage in _cellHighlightImages)
		{
			cellHighlightImage.SetHighlightVisibilityBySlotIds(placeableSlotIds, emptyBagSlotIds);
		}
	}

	internal void ResetBagSlotHighlights()
	{
		foreach (BagCellHighlightImage cellHighlightImage in _cellHighlightImages)
		{
			cellHighlightImage.ResetSlotHighlights();
		}
	}

	internal override void SetStoreInGridType(Enums.Backpack.GridType gridType, bool updateLines = true)
	{
		base.SetStoreInGridType(gridType, updateLines);
		_bagTooltipTrigger.enabled = base.StoredInGridType != Enums.Backpack.GridType.Backpack;
	}

	private void RegisterBaseEvents()
	{
		OnRotationChanged = (EventHandler)Delegate.Combine(OnRotationChanged, new EventHandler(Base_OnRotationChanged));
		base.OnSizeCalculated += Base_OnSizeCalculated;
		base.OnOwnerChanged += DraggableBag_OnOwnerChanged;
	}

	private void DraggableBag_OnOwnerChanged(object sender, EventArgs e)
	{
		_bagTooltipTrigger.ChangeOwner(base.Owner);
	}

	private void Base_OnSizeCalculated(object sender, DraggableSizeCalculatedEventArgs e)
	{
		RectTransform component = _highlightCellsParent.GetComponent<RectTransform>();
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)e.Width * 48f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)e.Height * 48f);
		AddHighlightCells(e);
	}

	private void AddHighlightCells(DraggableSizeCalculatedEventArgs e)
	{
		int num = e.Height * e.Width;
		for (int i = 0; i < num; i++)
		{
			BagCellHighlightImage bagCellHighlightImage = UnityEngine.Object.Instantiate(_bagCellHighlightImagePrefab, _highlightCellsParent.transform, worldPositionStays: false);
			int rowIndex = i / e.Width;
			int columnIndex = i % e.Width;
			bool canHighlight = GetCanHighlight(rowIndex, columnIndex);
			bagCellHighlightImage.Init(canHighlight, rowIndex, columnIndex);
			_cellHighlightImages.Add(bagCellHighlightImage);
		}
	}

	private bool GetCanHighlight(int rowIndex, int columnIndex)
	{
		int index = rowIndex * 10 + columnIndex;
		return base._placeableInfo.ItemSizeInfo[index] == Enums.Backpack.ItemSizeCellType.CellContainsPlacable;
	}

	private void Base_OnRotationChanged(object sender, EventArgs e)
	{
		CalculatePlaceable();
		Drag();
	}

	private void HighlightInvalidSlots(BackpackVisualGrid visualGrid, List<int> invalidSlotIds)
	{
		foreach (int invalidSlotId in invalidSlotIds)
		{
			visualGrid.ShowCellStatusColor(invalidSlotId, Constants.Colors.InvalidBackpackPlacementColor);
		}
	}

	private void HighlightValidSlots(BackpackVisualGrid visualGrid, List<int> itemSlotIds)
	{
		foreach (int itemSlotId in itemSlotIds)
		{
			visualGrid.ShowCellStatusColor(itemSlotId, Constants.Colors.ValidBackpackPlacementColor);
		}
	}

	internal override bool CanDrag(bool showCannotAffordUI = false)
	{
		bool num = SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().CanMoveBag(BagInstance);
		bool flag = base.CanDrag(showCannotAffordUI);
		return num && flag;
	}

	internal override void BeginDrag()
	{
		RemoveBagFromGrid();
		SetImageAlpha(1f);
		base.BeginDrag();
		_isDragging = true;
	}

	internal override void Drag()
	{
		SingletonController<BackpackController>.Instance.ToggleStorageChest(SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().GridType == Enums.Backpack.GridType.Storage, playAudio: false);
		BackpackVisualGrid hoveredVisualGrid = SingletonController<BackpackController>.Instance.GetHoveredVisualGrid();
		HoveredSlotInfo hoveredSlotInfo = SingletonController<BackpackController>.Instance.HoveredSlotProvider.GetHoveredSlotInfo();
		if (hoveredSlotInfo.Equals(HoveredSlotInfo.None))
		{
			hoveredVisualGrid.HideAllCellStatusColors();
			return;
		}
		List<int> slotIdsToPlaceItemIn = SingletonController<BackpackController>.Instance.GetHoveredGridSlotCalculator().GetSlotIdsToPlaceItemIn(base._placeableInfo, hoveredSlotInfo);
		if (!CanPlaceEntireItemInCells(slotIdsToPlaceItemIn))
		{
			hoveredVisualGrid.HideAllCellStatusColors();
		}
		else
		{
			UpdateCellHighlights(hoveredVisualGrid, slotIdsToPlaceItemIn);
		}
	}

	private List<int> UpdateCellHighlights(BackpackVisualGrid visualGrid, List<int> itemSlotIds)
	{
		List<int> invalidCellIds = new List<int>();
		visualGrid.HideAllCellStatusColors();
		if (SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage().CanPlaceBag(BagInstance, itemSlotIds, out invalidCellIds))
		{
			HighlightValidSlots(visualGrid, itemSlotIds);
		}
		else
		{
			HighlightValidSlots(visualGrid, itemSlotIds);
			HighlightInvalidSlots(visualGrid, invalidCellIds);
		}
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
		List<int> slotIdsToPlaceItemIn = SingletonController<BackpackController>.Instance.GetHoveredGridSlotCalculator().GetSlotIdsToPlaceItemIn(base._placeableInfo, hoveredSlotInfo);
		if (!CanPlaceEntireItemInCells(slotIdsToPlaceItemIn))
		{
			RevertDrop();
			return false;
		}
		IBackpackStorage hoveredBackpackStorage = SingletonController<BackpackController>.Instance.GetHoveredBackpackStorage();
		if (hoveredBackpackStorage.CanPlaceBag(BagInstance, slotIdsToPlaceItemIn, out var _))
		{
			if (!TryBuyIfNeeded())
			{
				RevertDrop();
				return false;
			}
			hoveredBackpackStorage.PlaceBag(BagInstance, slotIdsToPlaceItemIn);
			SetBagHighlightSlotIds(slotIdsToPlaceItemIn);
			base.StartItemSlotids = slotIdsToPlaceItemIn;
			SetStoreInGridType(hoveredBackpackStorage.GridType);
			CenterDraggableOnItemSlots(slotIdsToPlaceItemIn);
			UpdateTooltip();
			if (base.StoredInGridType == Enums.Backpack.GridType.Backpack)
			{
				SetImageAlpha(0.3f);
				_bagImage.sprite = BagInstance.BagSO.BackpackImage;
			}
			else
			{
				_bagImage.sprite = BagInstance.BagSO.BackpackImageWithoutSlots;
			}
			base.transform.SetParent(SingletonController<BackpackController>.Instance.BoughtBagsParentTransform);
			ResetScaleAndZIndex();
			return true;
		}
		RevertDrop();
		return false;
	}

	private void UpdateTooltip()
	{
		_bagTooltipTrigger.CanShowTooltip = base.Owner == Enums.Backpack.DraggableOwner.Shop || CanDrag();
	}

	private void SetBagHighlightSlotIds(List<int> itemSlotIds)
	{
		List<BagCellHighlightImage> rotatedHighlightImages = GetRotatedHighlightImages();
		for (int i = 0; i < itemSlotIds.Count; i++)
		{
			rotatedHighlightImages.Where((BagCellHighlightImage c) => c.CanHighlight).ToList()[i].SetSlotId(itemSlotIds[i]);
		}
	}

	private List<BagCellHighlightImage> GetRotatedHighlightImages()
	{
		return base.CurrentRotation switch
		{
			Enums.Backpack.ItemRotation.Rotation0 => _cellHighlightImages, 
			Enums.Backpack.ItemRotation.Rotation90 => (from c in _cellHighlightImages
				orderby c.HighlightImageColumnIndex, c.HighlightImageRowIndex descending
				select c).ToList(), 
			Enums.Backpack.ItemRotation.Rotation180 => (from c in _cellHighlightImages
				orderby c.HighlightImageRowIndex descending, c.HighlightImageColumnIndex descending
				select c).ToList(), 
			Enums.Backpack.ItemRotation.Rotation270 => (from c in _cellHighlightImages
				orderby c.HighlightImageColumnIndex descending, c.HighlightImageRowIndex
				select c).ToList(), 
			_ => _cellHighlightImages, 
		};
	}

	internal override void EndDrag(bool dropWasSuccess)
	{
		SingletonController<BackpackController>.Instance.GetHoveredVisualGrid().HideAllCellStatusColors();
		base.EndDrag(dropWasSuccess);
		_isDragging = false;
	}

	internal override void MoveToStorage()
	{
		if (base.StoredInGridType != Enums.Backpack.GridType.Storage && CanDrag())
		{
			SingletonController<BackpackController>.Instance.MoveToStorage(new List<ItemInstance>(), new List<WeaponInstance>(), new List<BagInstance> { BagInstance });
		}
	}

	internal override void RevertDrop()
	{
		RotateBackToPreviousRotation();
		PlaceBagBackIntoGrid();
		ReturnToStartOfDrag();
	}

	private void PlaceBagBackIntoGrid()
	{
		if (base.StartItemSlotids.Any())
		{
			SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).PlaceBag(BagInstance, base.StartItemSlotids);
		}
	}

	private void RemoveBagFromGrid()
	{
		SingletonController<BackpackController>.Instance.GetBackpackStorageByGridType(base.StoredInGridType).RemoveBag(BagInstance);
	}

	internal override void PointerEnter()
	{
		base.PointerEnter();
		if (CanInteract)
		{
			UpdateTooltip();
			if (CanDrag())
			{
				SetImageAlpha(1f);
				_bagImage.sprite = BagInstance.BagSO.BackpackImage;
			}
		}
	}

	internal override void PointerExit()
	{
		base.PointerExit();
		if (!CanInteract)
		{
			return;
		}
		UpdateTooltip();
		if (!_isDragging)
		{
			if (base.StoredInGridType == Enums.Backpack.GridType.Backpack && base.Owner == Enums.Backpack.DraggableOwner.Player)
			{
				SetImageAlpha(0.3f);
				_bagImage.sprite = BagInstance.BagSO.BackpackImage;
			}
			else
			{
				SetImageAlpha(1f);
				_bagImage.sprite = BagInstance.BagSO.BackpackImageWithoutSlots;
			}
		}
	}

	internal override void PointerMove()
	{
		base.PointerMove();
		UpdateTooltip();
	}
}
