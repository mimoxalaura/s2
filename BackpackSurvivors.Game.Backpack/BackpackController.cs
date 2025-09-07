using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Adventure.Interfaces;
using BackpackSurvivors.Game.Backpack.Events;
using BackpackSurvivors.Game.Backpack.Highlighting;
using BackpackSurvivors.Game.Backpack.Interfaces;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.Merging;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Backpack;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.Game.Stats;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.BackpackVFX;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Tooltip;
using QFSW.QC;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class BackpackController : BaseSingletonModalUIController<BackpackController>
{
	internal delegate void DraggableDroppedHandler(object sender, DraggableDroppedEventArgs e);

	internal delegate void DraggableSoldHandler(object sender, EventArgs e);

	internal delegate void DraggableDraggingHandler(object sender, DraggableDraggingEventArgs e);

	[SerializeField]
	private Canvas _canvas;

	[SerializeField]
	private Transform _boughtBagsParentTransform;

	[SerializeField]
	private Transform _boughtItemsAndWeaponsParentTransform;

	[SerializeField]
	private Image _storageChestImage;

	[SerializeField]
	private Sprite _storageChestOpen;

	[SerializeField]
	private Sprite _storageChestClose;

	[SerializeField]
	private Canvas _duringDragCanvas;

	[SerializeField]
	internal Transform DuringDragParentTransform;

	[SerializeField]
	private BackpackTutorialController _backpackTutorialController;

	[SerializeField]
	private int _maximumShopReservations;

	[Header("VFX")]
	[SerializeField]
	private BackpackDropItemVFX _placementVfxPrefab;

	[Header("VFX")]
	[SerializeField]
	private GameObject _PauseMenuUI;

	[SerializeField]
	private Transform _backpackContainerTransform;

	[SerializeField]
	private Transform _pauseContainerTransform;

	[SerializeField]
	private GameObject _bagsParent;

	[SerializeField]
	private GameObject _itemsParent;

	[SerializeField]
	private Transform _buttonCloseTarget;

	[SerializeField]
	private Transform _buttonOpenTarget;

	[SerializeField]
	private GameObject _buttonOpenClose;

	[SerializeField]
	private GameObject _backpackTitleBox;

	[SerializeField]
	private GameObject _pauseMenuCanvas;

	internal bool AllowDragging = true;

	internal int UsedShopRerolls;

	internal int UsedShopBanishes;

	private Vector2 _bagsOriginalPosition;

	private Vector2 _itemsOriginalPosition;

	private bool _opened = true;

	internal GameObject PauseMenuCanvas => _pauseMenuCanvas;

	internal Transform BoughtBagsParentTransform => _boughtBagsParentTransform;

	internal Transform BoughtItemsAndWeaponsParentTransform => _boughtItemsAndWeaponsParentTransform;

	internal BackpackStorage BackpackStorage { get; private set; }

	internal SlotCalculator BackpackSlotCalculator { get; private set; }

	internal HoveredSlotProvider HoveredSlotProvider { get; private set; }

	internal OutOfBackpackStorage OutOfBackpackStorage { get; private set; }

	internal SlotCalculator OutOfBackpackSlotCalculator { get; private set; }

	internal BackpackVisualGrid BackpackVisualGrid { get; private set; }

	internal BackpackVisualGrid OutOfBackpackVisualGrid { get; private set; }

	internal BagCellHighlightController BagCellHighlightController { get; private set; }

	internal CountController CountController { get; private set; }

	internal int MaximumShopReservations => _maximumShopReservations;

	internal BaseDraggable CurrentlyHoveredBaseDraggable { get; private set; }

	internal Canvas Canvas => _canvas;

	internal event DraggableDroppedHandler OnDraggableDropped;

	internal event DraggableSoldHandler OnDraggableSold;

	internal event DraggableDraggingHandler OnDraggableDragging;

	internal event Action OnUIShown;

	private void Start()
	{
		base.IsInitialized = true;
	}

	public override void Clear()
	{
		ClearBackpack();
		CountController.UpdateCounts();
	}

	public override void ClearAdventure()
	{
		ClearBackpack();
		CountController.UpdateCounts();
		UsedShopRerolls = 0;
		UsedShopBanishes = 0;
	}

	public void HideAllVFX()
	{
		BaseDraggable[] array = UnityEngine.Object.FindObjectsByType<BaseDraggable>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].HideVfxImage();
		}
	}

	[Command("backpack.clear", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void ClearBackpack()
	{
		BackpackStorage.ClearStorage();
		OutOfBackpackStorage.ClearStorage();
		ClearBoughtTransform(_boughtItemsAndWeaponsParentTransform);
		ClearBoughtTransform(_boughtBagsParentTransform);
	}

	private void ClearBoughtTransform(Transform boughtParentTransform)
	{
		BaseDraggable[] componentsInChildren = boughtParentTransform.GetComponentsInChildren<BaseDraggable>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	internal Dictionary<Enums.PlaceableTag, float> GetBackpackAndStorageAffinityTagsAndWeights()
	{
		List<Enums.PlaceableTag> obj = new List<Enums.PlaceableTag>
		{
			Enums.PlaceableTag.Fire,
			Enums.PlaceableTag.Cold,
			Enums.PlaceableTag.Lightning,
			Enums.PlaceableTag.Void,
			Enums.PlaceableTag.Poison,
			Enums.PlaceableTag.Energy,
			Enums.PlaceableTag.Holy,
			Enums.PlaceableTag.Melee,
			Enums.PlaceableTag.Ranged,
			Enums.PlaceableTag.Blunt,
			Enums.PlaceableTag.Piercing,
			Enums.PlaceableTag.Slashing
		};
		float backpackAffinityWeighting = SingletonController<GameDatabase>.Instance.GameDatabaseSO.BackpackAffinityWeighting;
		Dictionary<Enums.PlaceableTag, float> dictionary = new Dictionary<Enums.PlaceableTag, float>();
		List<WeaponInstance> list = new List<WeaponInstance>();
		list.AddRange(GetWeaponsFromBackpack());
		list.AddRange(GetWeaponsFromStorage());
		List<ItemInstance> list2 = new List<ItemInstance>();
		list2.AddRange(GetItemsFromBackpack());
		list2.AddRange(GetItemsFromStorage());
		foreach (Enums.PlaceableTag tagToWeighOn in obj)
		{
			int num = list.Count((WeaponInstance x) => x.CombinedPlaceableTags.HasFlag(tagToWeighOn));
			int num2 = list2.Count((ItemInstance x) => x.CombinedPlaceableTags.HasFlag(tagToWeighOn));
			dictionary.Add(tagToWeighOn, (float)(num + num2) * backpackAffinityWeighting);
		}
		return dictionary;
	}

	internal List<WeaponInstance> GetWeaponsFromBackpack()
	{
		return BackpackStorage.GetWeaponsInBackpack();
	}

	internal List<ItemInstance> GetItemsFromBackpack()
	{
		return BackpackStorage.GetItemsInBackpack();
	}

	internal List<WeaponInstance> GetWeaponsFromStorage()
	{
		return OutOfBackpackStorage.GetWeaponsInStorage();
	}

	internal List<ItemInstance> GetItemsFromStorage()
	{
		return OutOfBackpackStorage.GetItemsInStorage();
	}

	internal List<BagInstance> GetBagsFromBackpack()
	{
		return BackpackStorage.GetBagsInBackpack();
	}

	internal List<BagInstance> GetBagsFromStorage()
	{
		return OutOfBackpackStorage.GetBagsInBackpack();
	}

	internal IBackpackStorage GetHoveredBackpackStorage()
	{
		if (HoveredSlotProvider.IsHoveringOverBackpackGrid())
		{
			return BackpackStorage;
		}
		if (HoveredSlotProvider.IsHoveringOverStorageGrid())
		{
			return OutOfBackpackStorage;
		}
		return BackpackStorage;
	}

	internal IBackpackStorage GetBackpackStorageByGridType(Enums.Backpack.GridType gridType)
	{
		return gridType switch
		{
			Enums.Backpack.GridType.Backpack => BackpackStorage, 
			Enums.Backpack.GridType.Storage => OutOfBackpackStorage, 
			_ => null, 
		};
	}

	internal void UpdateCurrentlyHoveredDraggable(BaseDraggable draggable)
	{
		CurrentlyHoveredBaseDraggable = draggable;
	}

	internal SlotCalculator GetHoveredGridSlotCalculator()
	{
		if (HoveredSlotProvider.IsHoveringOverBackpackGrid())
		{
			return BackpackSlotCalculator;
		}
		if (HoveredSlotProvider.IsHoveringOverStorageGrid())
		{
			return OutOfBackpackSlotCalculator;
		}
		return BackpackSlotCalculator;
	}

	internal BackpackVisualGrid GetHoveredVisualGrid()
	{
		if (HoveredSlotProvider.IsHoveringOverBackpackGrid())
		{
			OutOfBackpackVisualGrid.HideAllCellStatusColors();
			return BackpackVisualGrid;
		}
		if (HoveredSlotProvider.IsHoveringOverStorageGrid())
		{
			BackpackVisualGrid.HideAllCellStatusColors();
			return OutOfBackpackVisualGrid;
		}
		OutOfBackpackVisualGrid.HideAllCellStatusColors();
		return BackpackVisualGrid;
	}

	internal BackpackVisualGrid GetVisualGridByStorageType(Enums.Backpack.GridType gridType)
	{
		if (gridType == Enums.Backpack.GridType.Storage)
		{
			return OutOfBackpackVisualGrid;
		}
		return BackpackVisualGrid;
	}

	internal void ClearAllVisualGridHighlights()
	{
		BackpackVisualGrid.HideAllCellOutlines();
		BackpackVisualGrid.HideAllStarImages();
		OutOfBackpackVisualGrid.HideAllCellOutlines();
		OutOfBackpackVisualGrid.HideAllStarImages();
	}

	internal void DraggableDropped(BaseDraggable draggable, bool success)
	{
		this.OnDraggableDropped?.Invoke(this, new DraggableDroppedEventArgs(draggable.DraggableType, draggable, success));
		SingletonController<StarLineController>.Instance.ClearHighlights();
		SingletonController<StarLineController>.Instance.ClearStarredLines();
	}

	internal void DraggableSold()
	{
		this.OnDraggableSold?.Invoke(this, new EventArgs());
	}

	internal void AddToCanvas(Transform transform)
	{
		transform.SetParent(_canvas.transform);
	}

	internal Vector2 GetCenterOfItemInGrid(BaseItemInstance itemInstance)
	{
		List<int> cellsPlaceableIsIn = BackpackStorage.GetCellsPlaceableIsIn(itemInstance);
		List<BackpackCell> backpackCells = BackpackVisualGrid.GetBackpackCells(cellsPlaceableIsIn);
		if (backpackCells.Count == 0)
		{
			return new Vector2(0f, 0f);
		}
		float x = backpackCells.Average((BackpackCell bpc) => bpc.transform.position.x);
		float y = backpackCells.Average((BackpackCell bpc) => bpc.transform.position.y);
		return new Vector2(x, y);
	}

	internal Vector2 GetCenterOfItemInGrid(ItemInstance itemInstance)
	{
		List<int> cellsPlaceableIsIn = BackpackStorage.GetCellsPlaceableIsIn(itemInstance);
		List<BackpackCell> backpackCells = BackpackVisualGrid.GetBackpackCells(cellsPlaceableIsIn);
		float x = backpackCells.Average((BackpackCell bpc) => bpc.transform.position.x);
		float y = backpackCells.Average((BackpackCell bpc) => bpc.transform.position.y);
		return new Vector2(x, y);
	}

	internal Vector2 GetCenterOfWeaponInGrid(WeaponInstance weaponInstance)
	{
		List<int> cellsPlaceableIsIn = BackpackStorage.GetCellsPlaceableIsIn(weaponInstance);
		List<BackpackCell> backpackCells = BackpackVisualGrid.GetBackpackCells(cellsPlaceableIsIn);
		float x = backpackCells.Average((BackpackCell bpc) => bpc.transform.position.x);
		float y = backpackCells.Average((BackpackCell bpc) => bpc.transform.position.y);
		return new Vector2(x, y);
	}

	internal void DraggableDragging(BaseDraggable draggable, bool effectIsPositive)
	{
		this.OnDraggableDragging?.Invoke(this, new DraggableDraggingEventArgs(draggable.DraggableType, draggable));
		HighlightAffectedWeaponsAndItems(draggable, animate: true, effectIsPositive);
	}

	internal void HighlightAffectedWeaponsAndItems(BaseDraggable draggable, bool animate, bool effectIsPositive)
	{
		if (draggable.StoredInGridType != Enums.Backpack.GridType.Storage)
		{
			SingletonController<StarLineController>.Instance.HighlightAffectedWeaponsAndItems(draggable, animate, effectIsPositive);
		}
	}

	internal void DrawLinesToAffectedWeaponsAndItems(BaseDraggable draggable, bool effectIsPositive)
	{
		if (draggable.StoredInGridType != Enums.Backpack.GridType.Storage)
		{
			SingletonController<StarLineController>.Instance.DrawLinesToAffectedWeaponsAndItems(draggable, effectIsPositive);
		}
	}

	internal void FadeAlphaCells(BackpackCell.BackpackCellAlpha backpackCellAlpha)
	{
		BackpackVisualGrid.FadeAlphaCells(backpackCellAlpha);
	}

	internal bool CanPlaceMergable(BaseItemSO mergeResult, List<BaseItemInstance> itemsToRemove, List<int> originalCellIds, out List<int> cellIdsToPlaceOn, out Enums.Backpack.ItemRotation possibleRotation)
	{
		bool addingWeapon = mergeResult.ItemType == Enums.PlaceableType.Weapon;
		if (SingletonController<StatController>.Instance.WeaponCapacityReached(addingWeapon))
		{
			cellIdsToPlaceOn = new List<int>();
			possibleRotation = Enums.Backpack.ItemRotation.Rotation0;
			return false;
		}
		BaseItemInstance baseItemInstance = new BaseItemInstance();
		baseItemInstance.SetBaseItemInstance(mergeResult);
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(mergeResult.ItemSize.GetItemSizeByRotation(value).ToList());
			foreach (int originalCellId in originalCellIds)
			{
				foreach (Tuple<bool, bool> item in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(originalCellId, item.Item1, item.Item2, Enums.Backpack.GridType.Backpack);
					List<int> slotIdsToPlaceItemIn = BackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if ((BackpackStorage.CanPlace(baseItemInstance, slotIdsToPlaceItemIn, out var invalidCellIds) || InvalidCellsWouldGetClearedByMerging(itemsToRemove, invalidCellIds)) && slotIdsToPlaceItemIn.Any())
					{
						cellIdsToPlaceOn = slotIdsToPlaceItemIn;
						possibleRotation = value;
						return true;
					}
				}
			}
		}
		cellIdsToPlaceOn = new List<int>();
		possibleRotation = Enums.Backpack.ItemRotation.Rotation0;
		return false;
	}

	internal bool CanPlaceMergableWeapon(WeaponSO weaponMergeResult, List<ItemInstance> itemsToRemove, List<WeaponInstance> weaponsToRemove, List<int> originalCellIds, out List<int> cellIdsToPlaceOn, out Enums.Backpack.ItemRotation possibleRotation)
	{
		bool addingWeapon = weaponsToRemove.Count() == 0;
		if (SingletonController<StatController>.Instance.WeaponCapacityReached(addingWeapon))
		{
			cellIdsToPlaceOn = new List<int>();
			possibleRotation = Enums.Backpack.ItemRotation.Rotation0;
			return false;
		}
		WeaponInstance weapon = new WeaponInstance(weaponMergeResult);
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(weaponMergeResult.ItemSize.GetItemSizeByRotation(value).ToList());
			foreach (int originalCellId in originalCellIds)
			{
				foreach (Tuple<bool, bool> item in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(originalCellId, item.Item1, item.Item2, Enums.Backpack.GridType.Backpack);
					List<int> slotIdsToPlaceItemIn = BackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (BackpackStorage.CanPlaceWeapon(weapon, slotIdsToPlaceItemIn, out var invalidCellIds) || InvalidCellsWouldGetClearedByMerging(itemsToRemove, weaponsToRemove, invalidCellIds))
					{
						cellIdsToPlaceOn = slotIdsToPlaceItemIn;
						possibleRotation = value;
						return true;
					}
				}
			}
		}
		cellIdsToPlaceOn = new List<int>();
		possibleRotation = Enums.Backpack.ItemRotation.Rotation0;
		return false;
	}

	internal bool CanPlaceMergableItem(ItemSO itemMergeResult, List<ItemInstance> itemsToRemove, List<WeaponInstance> weaponsToRemove, List<int> originalCellIds, out List<int> cellIdsToPlaceOn, out Enums.Backpack.ItemRotation possibleRotation)
	{
		ItemInstance item = new ItemInstance(itemMergeResult);
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(itemMergeResult.ItemSize.GetItemSizeByRotation(value).ToList());
			foreach (int originalCellId in originalCellIds)
			{
				foreach (Tuple<bool, bool> item2 in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(originalCellId, item2.Item1, item2.Item2, Enums.Backpack.GridType.Backpack);
					List<int> slotIdsToPlaceItemIn = BackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (BackpackStorage.CanPlaceItem(item, slotIdsToPlaceItemIn, out var invalidCellIds) || InvalidCellsWouldGetClearedByMerging(itemsToRemove, weaponsToRemove, invalidCellIds))
					{
						cellIdsToPlaceOn = slotIdsToPlaceItemIn;
						possibleRotation = value;
						return true;
					}
				}
			}
		}
		cellIdsToPlaceOn = new List<int>();
		possibleRotation = Enums.Backpack.ItemRotation.Rotation0;
		return false;
	}

	private bool InvalidCellsWouldGetClearedByMerging(List<BaseItemInstance> baseItemsToRemove, List<int> invalidCellIds)
	{
		List<int> cellsClearedByRemovingItems = new List<int>();
		List<int> cellsClearedByRemovingWeapons = new List<int>();
		foreach (BaseItemInstance item in baseItemsToRemove)
		{
			cellsClearedByRemovingItems.AddRange(BackpackStorage.GetCellsPlaceableIsIn(item));
		}
		return invalidCellIds.All((int ic) => cellsClearedByRemovingItems.Contains(ic) || cellsClearedByRemovingWeapons.Contains(ic));
	}

	private bool InvalidCellsWouldGetClearedByMerging(List<ItemInstance> itemsToRemove, List<WeaponInstance> weaponsToRemove, List<int> invalidCellIds)
	{
		List<int> cellsClearedByRemovingItems = new List<int>();
		List<int> cellsClearedByRemovingWeapons = new List<int>();
		foreach (ItemInstance item in itemsToRemove)
		{
			cellsClearedByRemovingItems.AddRange(BackpackStorage.GetCellsPlaceableIsIn(item));
		}
		foreach (WeaponInstance item2 in weaponsToRemove)
		{
			cellsClearedByRemovingItems.AddRange(BackpackStorage.GetCellsPlaceableIsIn(item2));
		}
		return invalidCellIds.All((int ic) => cellsClearedByRemovingItems.Contains(ic) || cellsClearedByRemovingWeapons.Contains(ic));
	}

	internal bool AddBagToBackpack(BagSO bagSO, List<int> cellIdsToPlaceOn, Enums.Backpack.ItemRotation rotation = Enums.Backpack.ItemRotation.Rotation0, bool showVfx = false, bool fromMerge = false)
	{
		new PlaceableInfo(bagSO.ItemSize.GetItemSizeByRotation(rotation).ToList());
		cellIdsToPlaceOn.Min((int s) => s);
		DraggableBag component = UnityEngine.Object.Instantiate(GameDatabaseHelper.GetDraggableBagPrefab(), _boughtBagsParentTransform).GetComponent<DraggableBag>();
		component.Enabled = true;
		component.Init(bagSO);
		component.SetRotation(rotation);
		component.ApplyRotation(animated: false);
		component.CenterDraggableOnItemSlots(cellIdsToPlaceOn, BackpackVisualGrid);
		component.SetPlayerOwned();
		component.transform.SetParent(_boughtBagsParentTransform, worldPositionStays: true);
		component.SetStartItemSlotids(cellIdsToPlaceOn);
		component.SetStoreInGridType(Enums.Backpack.GridType.Backpack);
		component.SetImageAlpha(0.3f);
		component.BagInstance.UpdateDroppedInBackpackTime();
		bool num = BackpackStorage.PlaceBag(component.BagInstance, cellIdsToPlaceOn);
		if (num && showVfx)
		{
			ShowPlacementVfx(component, fromMerge);
		}
		component.ResetScaleAndZIndex();
		return num;
	}

	internal bool AddItemToBackpack(ItemSO itemSO, List<int> cellIdsToPlaceOn, Enums.Backpack.ItemRotation rotation = Enums.Backpack.ItemRotation.Rotation0, bool showVfx = false, bool fromMerge = false)
	{
		PlaceableInfo placeable = new PlaceableInfo(itemSO.ItemSize.GetItemSizeByRotation(rotation).ToList());
		int topLeftSlotId = cellIdsToPlaceOn.Min((int s) => s);
		List<int> slotIdsToPlaceStarsIn = BackpackSlotCalculator.GetSlotIdsToPlaceStarsIn(placeable, topLeftSlotId);
		DraggableItem component = UnityEngine.Object.Instantiate(GameDatabaseHelper.GetDraggableItemPrefab(), _boughtItemsAndWeaponsParentTransform).GetComponent<DraggableItem>();
		component.Enabled = true;
		component.Init(itemSO);
		component.SetRotation(rotation);
		component.ApplyRotation(animated: false);
		component.CenterDraggableOnItemSlots(cellIdsToPlaceOn, BackpackVisualGrid);
		component.SetPlayerOwned();
		component.transform.SetParent(BoughtItemsAndWeaponsParentTransform, worldPositionStays: true);
		component.SetStartItemSlotids(cellIdsToPlaceOn);
		List<int> slotIdsToPlaceStarsIn2 = BackpackSlotCalculator.GetSlotIdsToPlaceStarsIn(placeable, cellIdsToPlaceOn.Min());
		component.SetStartStarSlotIds(slotIdsToPlaceStarsIn2);
		component.SetStoreInGridType(Enums.Backpack.GridType.Backpack);
		component.ItemInstance.UpdateDroppedInBackpackTime();
		bool num = BackpackStorage.PlaceItem(component.ItemInstance, cellIdsToPlaceOn, slotIdsToPlaceStarsIn);
		if (num && showVfx)
		{
			ShowPlacementVfx(component, fromMerge);
		}
		component.ResetScaleAndZIndex();
		return num;
	}

	internal bool AddWeaponToBackpack(WeaponSO weaponSO, List<int> cellIdsToPlaceOn, Enums.Backpack.ItemRotation rotation = Enums.Backpack.ItemRotation.Rotation0, bool showVfx = false, bool fromMerge = false)
	{
		PlaceableInfo placeable = new PlaceableInfo(weaponSO.ItemSize.GetItemSizeByRotation(rotation).ToList());
		int topLeftSlotId = cellIdsToPlaceOn.Min((int s) => s);
		List<int> slotIdsToPlaceStarsIn = BackpackSlotCalculator.GetSlotIdsToPlaceStarsIn(placeable, topLeftSlotId);
		DraggableWeapon component = UnityEngine.Object.Instantiate(GameDatabaseHelper.GetDraggableWeaponPrefab(), _boughtItemsAndWeaponsParentTransform).GetComponent<DraggableWeapon>();
		component.Enabled = true;
		component.Init(weaponSO);
		component.SetRotation(rotation);
		component.ApplyRotation(animated: false);
		component.CenterDraggableOnItemSlots(cellIdsToPlaceOn, BackpackVisualGrid);
		component.SetPlayerOwned();
		component.transform.SetParent(BoughtItemsAndWeaponsParentTransform, worldPositionStays: true);
		component.SetStartItemSlotids(cellIdsToPlaceOn);
		List<int> slotIdsToPlaceStarsIn2 = BackpackSlotCalculator.GetSlotIdsToPlaceStarsIn(placeable, cellIdsToPlaceOn.Min());
		component.SetStartStarSlotIds(slotIdsToPlaceStarsIn2);
		component.SetStoreInGridType(Enums.Backpack.GridType.Backpack);
		component.WeaponInstance.UpdateDroppedInBackpackTime();
		bool num = BackpackStorage.PlaceWeapon(component.WeaponInstance, cellIdsToPlaceOn, slotIdsToPlaceStarsIn);
		if (num && showVfx)
		{
			ShowPlacementVfx(component, fromMerge);
		}
		component.ResetScaleAndZIndex();
		return num;
	}

	internal bool CanPlaceBagInStorage(BagSO bagSO)
	{
		BagInstance bag = new BagInstance(bagSO);
		int num = 72;
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(bagSO.ItemSize.GetItemSizeByRotation(value).ToList());
			for (int i = 0; i < num; i++)
			{
				foreach (Tuple<bool, bool> item in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, item.Item1, item.Item2, Enums.Backpack.GridType.Storage);
					List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (OutOfBackpackStorage.CanPlaceBag(bag, slotIdsToPlaceItemIn, out var _))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	internal bool AddBagToStorage(BagSO bagSO, bool showVfx = false, bool fromMerge = false)
	{
		BagInstance bag = new BagInstance(bagSO);
		int num = 72;
		bool flag = false;
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(bagSO.ItemSize.GetItemSizeByRotation(value).ToList());
			for (int i = 0; i < num; i++)
			{
				foreach (Tuple<bool, bool> item in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, item.Item1, item.Item2, Enums.Backpack.GridType.Storage);
					List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (OutOfBackpackStorage.CanPlaceBag(bag, slotIdsToPlaceItemIn, out var _))
					{
						flag = true;
						DraggableBag component = UnityEngine.Object.Instantiate(GameDatabaseHelper.GetDraggableBagPrefab(), _boughtBagsParentTransform).GetComponent<DraggableBag>();
						component.Enabled = true;
						component.Init(bagSO);
						component.CenterDraggableOnItemSlots(slotIdsToPlaceItemIn, OutOfBackpackVisualGrid);
						component.SetRotation(value);
						component.ApplyRotation(animated: false);
						component.SetPlayerOwned();
						component.SetStoreInGridType(Enums.Backpack.GridType.Storage);
						component.transform.SetParent(BoughtBagsParentTransform);
						component.SetStartItemSlotids(slotIdsToPlaceItemIn);
						OutOfBackpackStorage.PlaceBag(component.BagInstance, slotIdsToPlaceItemIn);
						if (showVfx)
						{
							ShowPlacementVfx(component, fromMerge);
						}
						component.ResetScaleAndZIndex();
						return true;
					}
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning($"Could not place bag with Id: {bagSO.Id}");
		}
		return flag;
	}

	internal bool CanPlaceWeaponInStorage(WeaponSO weaponSO)
	{
		WeaponInstance weapon = new WeaponInstance(weaponSO);
		int num = 72;
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(weaponSO.ItemSize.GetItemSizeByRotation(value).ToList());
			for (int i = 0; i < num; i++)
			{
				foreach (Tuple<bool, bool> item in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, item.Item1, item.Item2, Enums.Backpack.GridType.Storage);
					List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (OutOfBackpackStorage.CanPlaceWeapon(weapon, slotIdsToPlaceItemIn, out var _))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	internal bool AddWeaponToStorage(WeaponSO weaponSO, bool showVfx = false, bool fromMerge = false, bool updateLines = true)
	{
		WeaponInstance weapon = new WeaponInstance(weaponSO);
		int num = 72;
		bool flag = false;
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(weaponSO.ItemSize.GetItemSizeByRotation(value).ToList());
			for (int i = 0; i < num; i++)
			{
				foreach (Tuple<bool, bool> item in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, item.Item1, item.Item2, Enums.Backpack.GridType.Storage);
					List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (OutOfBackpackStorage.CanPlaceWeapon(weapon, slotIdsToPlaceItemIn, out var _))
					{
						flag = true;
						DraggableWeapon component = UnityEngine.Object.Instantiate(GameDatabaseHelper.GetDraggableWeaponPrefab(), _boughtItemsAndWeaponsParentTransform).GetComponent<DraggableWeapon>();
						component.Enabled = true;
						component.Init(weaponSO);
						component.CenterDraggableOnItemSlots(slotIdsToPlaceItemIn, OutOfBackpackVisualGrid);
						component.SetRotation(value);
						component.ApplyRotation(animated: false);
						component.SetPlayerOwned();
						component.transform.SetParent(BoughtItemsAndWeaponsParentTransform);
						component.SetStartItemSlotids(slotIdsToPlaceItemIn);
						List<int> slotIdsToPlaceStarsIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceStarsIn(placeable, slotIdsToPlaceItemIn.Min());
						component.SetStartStarSlotIds(slotIdsToPlaceStarsIn);
						component.SetStoreInGridType(Enums.Backpack.GridType.Storage, updateLines);
						component.WeaponInstance.UpdateDroppedInBackpackTime();
						OutOfBackpackStorage.PlaceWeapon(component.WeaponInstance, slotIdsToPlaceItemIn, new List<int>());
						if (showVfx)
						{
							ShowPlacementVfx(component, fromMerge);
						}
						component.ResetScaleAndZIndex();
						return true;
					}
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning($"Could not place weapon with Id: {weaponSO.Id}");
		}
		return flag;
	}

	internal bool CanPlaceItemInStorage(ItemSO itemSO)
	{
		ItemInstance item = new ItemInstance(itemSO);
		int num = 72;
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(itemSO.ItemSize.GetItemSizeByRotation(value).ToList());
			for (int i = 0; i < num; i++)
			{
				foreach (Tuple<bool, bool> item2 in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, item2.Item1, item2.Item2, Enums.Backpack.GridType.Storage);
					List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (OutOfBackpackStorage.CanPlaceItem(item, slotIdsToPlaceItemIn, out var _))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	internal bool AddItemToStorage(ItemSO itemSO, bool showVfx = false, bool fromMerge = false, bool updateLines = true)
	{
		ItemInstance item = new ItemInstance(itemSO);
		int num = 72;
		bool flag = false;
		List<Tuple<bool, bool>> hoveredSlotInfoSublocations = GetHoveredSlotInfoSublocations();
		foreach (Enums.Backpack.ItemRotation value in Enum.GetValues(typeof(Enums.Backpack.ItemRotation)))
		{
			PlaceableInfo placeable = new PlaceableInfo(itemSO.ItemSize.GetItemSizeByRotation(value).ToList());
			for (int i = 0; i < num; i++)
			{
				foreach (Tuple<bool, bool> item2 in hoveredSlotInfoSublocations)
				{
					HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, item2.Item1, item2.Item2, Enums.Backpack.GridType.Storage);
					List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
					if (OutOfBackpackStorage.CanPlaceItem(item, slotIdsToPlaceItemIn, out var _))
					{
						flag = true;
						DraggableItem component = UnityEngine.Object.Instantiate(GameDatabaseHelper.GetDraggableItemPrefab(), _boughtItemsAndWeaponsParentTransform).GetComponent<DraggableItem>();
						component.Enabled = true;
						component.Init(itemSO);
						component.CenterDraggableOnItemSlots(slotIdsToPlaceItemIn, OutOfBackpackVisualGrid);
						component.SetRotation(value);
						component.ApplyRotation(animated: false);
						component.SetPlayerOwned();
						component.transform.SetParent(BoughtItemsAndWeaponsParentTransform);
						component.SetStartItemSlotids(slotIdsToPlaceItemIn);
						List<int> slotIdsToPlaceStarsIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceStarsIn(placeable, slotIdsToPlaceItemIn.Min());
						component.SetStartStarSlotIds(slotIdsToPlaceStarsIn);
						component.SetStoreInGridType(Enums.Backpack.GridType.Storage, updateLines);
						component.ItemInstance.UpdateDroppedInBackpackTime();
						OutOfBackpackStorage.PlaceItem(component.ItemInstance, slotIdsToPlaceItemIn, new List<int>());
						if (showVfx)
						{
							ShowPlacementVfx(component, fromMerge);
						}
						component.ResetScaleAndZIndex();
						return true;
					}
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning($"Could not place item with Id: {itemSO.Id}");
		}
		return flag;
	}

	private List<Tuple<bool, bool>> GetHoveredSlotInfoSublocations()
	{
		return new List<Tuple<bool, bool>>
		{
			new Tuple<bool, bool>(item1: false, item2: false),
			new Tuple<bool, bool>(item1: true, item2: false),
			new Tuple<bool, bool>(item1: false, item2: true),
			new Tuple<bool, bool>(item1: true, item2: true)
		};
	}

	private void ShowPlacementVfx(DraggableItem item, bool fromMerge = false)
	{
		UnityEngine.Object.Instantiate(_placementVfxPrefab, item.transform).Init(item.ItemInstance.ItemSO, item.VFXImage, 2f, fromMerge);
	}

	private void ShowPlacementVfx(DraggableWeapon weapon, bool fromMerge = false)
	{
		UnityEngine.Object.Instantiate(_placementVfxPrefab, weapon.transform).Init(weapon.WeaponInstance.BaseWeaponSO, weapon.VFXImage, 2f, fromMerge);
	}

	private void ShowPlacementVfx(DraggableBag bag, bool fromMerge = false)
	{
		UnityEngine.Object.Instantiate(_placementVfxPrefab, bag.transform).Init(bag.BagInstance.BagSO, bag.VFXImage, 2f, fromMerge);
	}

	private List<int> GetItemSlotsToPlaceItemIn(ItemInstance itemInstance, bool showVfx = false)
	{
		PlaceableInfo placeable = new PlaceableInfo(itemInstance.ItemSO.ItemSize.SizeInfo.ToList());
		int num = 72;
		for (int i = 0; i < num; i++)
		{
			HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, isHoveredSlotOnRight: false, isHoveredSlotOnBottom: false, Enums.Backpack.GridType.Storage);
			List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
			if (OutOfBackpackStorage.CanPlaceItem(itemInstance, slotIdsToPlaceItemIn, out var _))
			{
				return slotIdsToPlaceItemIn;
			}
		}
		return new List<int>();
	}

	private List<int> GetWeaponSlotsToPlaceWeaponIn(WeaponInstance weaponInstance)
	{
		PlaceableInfo placeable = new PlaceableInfo(weaponInstance.BaseItemSO.ItemSize.SizeInfo.ToList());
		int num = 72;
		for (int i = 0; i < num; i++)
		{
			HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, isHoveredSlotOnRight: false, isHoveredSlotOnBottom: false, Enums.Backpack.GridType.Storage);
			List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
			if (OutOfBackpackStorage.CanPlaceWeapon(weaponInstance, slotIdsToPlaceItemIn, out var _))
			{
				return slotIdsToPlaceItemIn;
			}
		}
		return new List<int>();
	}

	private List<int> GetBagSlotsToPlaceBagIn(BagInstance bagInstance)
	{
		PlaceableInfo placeable = new PlaceableInfo(bagInstance.BaseItemSO.ItemSize.SizeInfo.ToList());
		int num = 72;
		for (int i = 0; i < num; i++)
		{
			HoveredSlotInfo hoveredSlotInfo = new HoveredSlotInfo(i, isHoveredSlotOnRight: false, isHoveredSlotOnBottom: false, Enums.Backpack.GridType.Storage);
			List<int> slotIdsToPlaceItemIn = OutOfBackpackSlotCalculator.GetSlotIdsToPlaceItemIn(placeable, hoveredSlotInfo);
			if (OutOfBackpackStorage.CanPlaceBag(bagInstance, slotIdsToPlaceItemIn, out var _))
			{
				return slotIdsToPlaceItemIn;
			}
		}
		return new List<int>();
	}

	internal void LockBackpackCells(List<int> lockedRowIds, List<int> lockedColumnIds)
	{
		List<int> cellIndexes = BackpackStorage.LockCells(lockedRowIds, lockedColumnIds);
		List<int> list = new List<int>();
		for (int i = 0; i < 144; i++)
		{
			list.Add(i);
		}
		BackpackVisualGrid.ToggleLockedCell(list, locked: false);
		BackpackVisualGrid.ToggleLockedCell(cellIndexes, locked: true);
		BackpackVisualGrid.SetLockedCellBorder(lockedRowIds, lockedColumnIds);
		BackpackVisualGrid.SetLockedColumnRowOverlay(lockedRowIds, lockedColumnIds);
	}

	private void Init()
	{
		BackpackStorage = new BackpackStorage(12, 12, Enums.Backpack.GridType.Backpack);
		BackpackSlotCalculator = new SlotCalculator(12);
		HoveredSlotProvider = new HoveredSlotProvider();
		OutOfBackpackStorage = new OutOfBackpackStorage(12, 6, Enums.Backpack.GridType.Storage);
		OutOfBackpackSlotCalculator = new SlotCalculator(12);
		BackpackVisualGrid = GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Backpack);
		OutOfBackpackVisualGrid = GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Storage);
		BagCellHighlightController = new BagCellHighlightController(BackpackStorage);
		CountController = new CountController();
	}

	private BackpackVisualGrid GetBackpackVisualGridByGridType(Enums.Backpack.GridType gridType)
	{
		return UnityEngine.Object.FindObjectsByType<BackpackVisualGrid>(FindObjectsSortMode.None).FirstOrDefault((BackpackVisualGrid vg) => vg.GridType == gridType);
	}

	public override void AfterBaseAwake()
	{
		Init();
	}

	internal WeaponInstance GetRarestWeaponInBackpack()
	{
		return (from x in GetWeaponsFromBackpack()
			orderby x.ItemRarity descending
			select x).FirstOrDefault();
	}

	internal ItemInstance GetRarestArmorInBackpack()
	{
		return (from x in GetItemsFromBackpack()
			where x.ItemSubtype == Enums.PlaceableItemSubtype.BodyArmor
			orderby x.ItemSO.ItemRarity descending
			select x).FirstOrDefault();
	}

	internal ItemInstance GetRarestShieldInBackpack()
	{
		return (from x in GetItemsFromBackpack()
			where x.ItemSubtype == Enums.PlaceableItemSubtype.Shield
			orderby x.ItemSO.ItemRarity descending
			select x).FirstOrDefault();
	}

	internal ItemInstance GetRarestHelmetInBackpack()
	{
		return (from x in GetItemsFromBackpack()
			where x.ItemSubtype == Enums.PlaceableItemSubtype.Headwear
			orderby x.ItemSO.ItemRarity descending
			select x).FirstOrDefault();
	}

	internal ItemInstance GetRarestGlovesInBackpack()
	{
		return (from x in GetItemsFromBackpack()
			where x.ItemSubtype == Enums.PlaceableItemSubtype.Gloves
			orderby x.ItemSO.ItemRarity descending
			select x).FirstOrDefault();
	}

	internal ItemInstance GetRarestBootsInBackpack()
	{
		return (from x in GetItemsFromBackpack()
			where x.ItemSubtype == Enums.PlaceableItemSubtype.Boots
			orderby x.ItemSO.ItemRarity descending
			select x).FirstOrDefault();
	}

	[Command("backpack.open", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		base.OpenUI();
		SingletonController<MergeController>.Instance.SetCamerasEnabled(enabled: true);
		SingletonController<StarLineController>.Instance.SetCamerasEnabled(enabled: true);
		_canvas.gameObject.SetActive(value: true);
		_duringDragCanvas.gameObject.SetActive(value: true);
		SingletonController<GameController>.Instance.DestroyProjectileAssets();
		if (!SingletonController<TutorialController>.Instance.GetSaveState().ShownBackpackTutorial)
		{
			SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownBackpackTutorial = true;
			_backpackTutorialController.StartTutorial();
		}
		SingletonController<GameController>.Instance.Player.RefreshVisuals();
		this.OnUIShown?.Invoke();
	}

	[Command("backpack.close", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void CloseUI()
	{
		base.CloseUI();
		_canvas.gameObject.SetActive(value: false);
		_duringDragCanvas.gameObject.SetActive(value: false);
		SingletonController<MergeController>.Instance.SetCamerasEnabled(enabled: false);
		SingletonController<StarLineController>.Instance.SetCamerasEnabled(enabled: false);
	}

	private void MoveBackpackBagAndItemsParent(bool toPauseMenuLocation)
	{
		if (toPauseMenuLocation)
		{
			_bagsParent.transform.SetParent(_pauseContainerTransform, worldPositionStays: false);
			_itemsParent.transform.SetParent(_pauseContainerTransform, worldPositionStays: false);
			_bagsOriginalPosition = _bagsParent.transform.localPosition;
			_bagsParent.transform.localScale = new Vector3(1f, 1f, 1f);
			_bagsParent.transform.localPosition = new Vector3(0f, 0f);
			_itemsOriginalPosition = _itemsParent.transform.localPosition;
			_itemsParent.transform.localScale = new Vector3(1f, 1f, 1f);
			_itemsParent.transform.localPosition = new Vector3(0f, 0f);
		}
		else
		{
			_bagsParent.transform.SetParent(_backpackContainerTransform, worldPositionStays: false);
			_bagsParent.transform.localScale = new Vector3(1f, 1f, 1f);
			_bagsParent.transform.localPosition = _bagsOriginalPosition;
			_itemsParent.transform.SetParent(_backpackContainerTransform, worldPositionStays: false);
			_itemsParent.transform.localScale = new Vector3(1f, 1f, 1f);
			_itemsParent.transform.localPosition = _itemsOriginalPosition;
		}
	}

	public void OpenPauseMenuUI()
	{
		AllowDragging = false;
		MoveBackpackBagAndItemsParent(toPauseMenuLocation: true);
		if (_opened)
		{
			LeanTween.cancel(_PauseMenuUI);
			LeanTween.scaleX(_PauseMenuUI, 1f, 0.3f).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	public void ClosePauseMenuUI()
	{
		AllowDragging = true;
		SingletonController<StarLineController>.Instance.ClearStarredLines();
		SingletonController<TooltipController>.Instance.Hide(null);
		MoveBackpackBagAndItemsParent(toPauseMenuLocation: false);
		if (_opened)
		{
			LeanTween.cancel(_PauseMenuUI);
			LeanTween.scaleX(_PauseMenuUI, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		SingletonController<MergeController>.Instance.ClearCompleteMergableLines();
		SingletonController<MergeController>.Instance.ClearPotentialLines();
		SingletonController<MergeController>.Instance.ClearIncompleteMergableLines();
		SingletonController<StarLineController>.Instance.ClearStarredLines();
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.Backpack;
	}

	internal void ToggleStorageChest(bool open, bool playAudio)
	{
		if (open)
		{
			_storageChestImage.sprite = _storageChestOpen;
		}
		else
		{
			_storageChestImage.sprite = _storageChestClose;
		}
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	internal void HighlightBagSlotsWherePlaceableWouldBePlaced(List<int> itemSlotIds)
	{
		if (GetHoveredBackpackStorage().GridType != Enums.Backpack.GridType.Storage)
		{
			BagCellHighlightController.HighlightBagSlots(itemSlotIds);
		}
	}

	internal void ResetBagSlotHighlights()
	{
		BagCellHighlightController.ResetBagSlotHighlights();
	}

	internal void ClearAllStarImages()
	{
		GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Backpack).HideAllStarImages();
		GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Storage).HideAllStarImages();
	}

	public override bool AudioOnOpen()
	{
		return false;
	}

	public override bool AudioOnClose()
	{
		return false;
	}

	internal bool IsCurrentBackpackCombatViable()
	{
		bool flag = false;
		foreach (WeaponInstance item in BackpackStorage.GetWeaponsInBackpack())
		{
			if (item.DamageInstance.BaseMinDamage > 0f)
			{
				flag = true;
			}
		}
		bool flag2 = true;
		flag2 = SingletonController<GameController>.Instance.Player.CalculatedStats.TryGet(Enums.ItemStatType.Spiked, 0f) > 0f;
		return flag || flag2;
	}

	internal DraggableItem GetDraggableItem(ItemInstance itemInstance)
	{
		return (from i in _boughtItemsAndWeaponsParentTransform.GetComponentsInChildren<DraggableItem>()
			where i.ItemInstance.Equals(itemInstance)
			select i).FirstOrDefault();
	}

	internal DraggableWeapon GetDraggableWeapon(WeaponInstance weaponInstance)
	{
		return (from w in _boughtItemsAndWeaponsParentTransform.GetComponentsInChildren<DraggableWeapon>()
			where w.WeaponInstance.Equals(weaponInstance)
			select w).FirstOrDefault();
	}

	internal DraggableBag GetDraggableBag(BagInstance bagInstance)
	{
		return (from b in _boughtBagsParentTransform.GetComponentsInChildren<DraggableBag>()
			where b.BagInstance.Equals(bagInstance)
			select b).FirstOrDefault();
	}

	internal BaseDraggable GetDraggableBase(BaseItemInstance baseItemInstance)
	{
		return (from b in _boughtBagsParentTransform.GetComponentsInChildren<BaseDraggable>()
			where b.BaseItemInstance.Equals(baseItemInstance)
			select b).FirstOrDefault();
	}

	internal bool MoveToStorage(List<ItemInstance> itemsToMoveToStorage, List<WeaponInstance> weaponsToMoveToStorage, List<BagInstance> bagsToMoveToStorage)
	{
		foreach (ItemInstance item in itemsToMoveToStorage)
		{
			DraggableItem draggableItem = GetDraggableItem(item);
			IBackpackStorage backpackStorageByGridType = GetBackpackStorageByGridType(draggableItem.StoredInGridType);
			List<int> itemSlotsToPlaceItemIn = GetItemSlotsToPlaceItemIn(item);
			if (itemSlotsToPlaceItemIn.Count == 0)
			{
				Debug.LogWarning("Cannot move item " + item.Name + " to storage");
				return false;
			}
			backpackStorageByGridType.RemoveItem(item);
			GetBackpackStorageByGridType(Enums.Backpack.GridType.Storage).PlaceItem(item, itemSlotsToPlaceItemIn, new List<int>());
			draggableItem.SetStartItemSlotids(itemSlotsToPlaceItemIn);
			draggableItem.ResetRotation();
			draggableItem.SetStoreInGridType(Enums.Backpack.GridType.Storage);
			draggableItem.CenterDraggableOnItemSlots(itemSlotsToPlaceItemIn, GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Storage));
			draggableItem.ResetScaleAndZIndex();
		}
		foreach (WeaponInstance item2 in weaponsToMoveToStorage)
		{
			DraggableWeapon draggableWeapon = GetDraggableWeapon(item2);
			IBackpackStorage backpackStorageByGridType2 = GetBackpackStorageByGridType(draggableWeapon.StoredInGridType);
			List<int> weaponSlotsToPlaceWeaponIn = GetWeaponSlotsToPlaceWeaponIn(item2);
			if (weaponSlotsToPlaceWeaponIn.Count == 0)
			{
				Debug.LogWarning("Cannot move weapon " + item2.Name + " to storage");
				return false;
			}
			backpackStorageByGridType2.RemoveWeapon(item2);
			GetBackpackStorageByGridType(Enums.Backpack.GridType.Storage).PlaceWeapon(item2, weaponSlotsToPlaceWeaponIn, new List<int>());
			draggableWeapon.SetStartItemSlotids(weaponSlotsToPlaceWeaponIn);
			draggableWeapon.ResetRotation();
			draggableWeapon.SetStoreInGridType(Enums.Backpack.GridType.Storage);
			draggableWeapon.CenterDraggableOnItemSlots(weaponSlotsToPlaceWeaponIn, GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Storage));
			draggableWeapon.ResetScaleAndZIndex();
		}
		foreach (BagInstance item3 in bagsToMoveToStorage)
		{
			DraggableBag draggableBag = GetDraggableBag(item3);
			IBackpackStorage backpackStorageByGridType3 = GetBackpackStorageByGridType(draggableBag.StoredInGridType);
			List<int> bagSlotsToPlaceBagIn = GetBagSlotsToPlaceBagIn(item3);
			if (bagSlotsToPlaceBagIn.Count == 0)
			{
				Debug.LogWarning("Cannot move bag " + item3.Name + " to storage");
				return false;
			}
			backpackStorageByGridType3.RemoveBag(item3);
			GetBackpackStorageByGridType(Enums.Backpack.GridType.Storage).PlaceBag(item3, bagSlotsToPlaceBagIn);
			draggableBag.SetStartItemSlotids(bagSlotsToPlaceBagIn);
			draggableBag.ResetRotation();
			draggableBag.SetStoreInGridType(Enums.Backpack.GridType.Storage);
			draggableBag.CenterDraggableOnItemSlots(bagSlotsToPlaceBagIn, GetBackpackVisualGridByGridType(Enums.Backpack.GridType.Storage));
			draggableBag.ResetScaleAndZIndex();
		}
		CountController.UpdateCounts();
		return true;
	}

	internal bool CanPlaceItemBackIntoStorage(ItemInstance itemInstance)
	{
		GetDraggableItem(itemInstance);
		return GetItemSlotsToPlaceItemIn(itemInstance).Count > 0;
	}

	internal bool CanPlaceWeaponBackIntoStorage(WeaponInstance weaponInstance)
	{
		GetDraggableWeapon(weaponInstance);
		return GetWeaponSlotsToPlaceWeaponIn(weaponInstance).Count > 0;
	}

	internal void ClearMoveToStorage()
	{
		BackpackStorage.ClearMoveToStorage();
	}

	[Command("backpack.Save", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void SaveCurrentBackpackState()
	{
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.SaveBackpackState();
		SingletonController<SaveGameController>.Instance.SaveProgression();
	}

	[Command("backpack.Load", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void LoadSavedBackpackState()
	{
		if (SingletonController<SaveGameController>.Instance.ActiveSaveGame.BackpackSaveState == null)
		{
			Debug.Log("No backpack state saved");
			return;
		}
		BackpackSaveState backpackSaveState = SingletonController<SaveGameController>.Instance.ActiveSaveGame.BackpackSaveState;
		ClearBackpack();
		foreach (BagInBackpack bag in backpackSaveState.BagsInBackpack)
		{
			BagSO bagSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableBags.FirstOrDefault((BagSO b) => b.Id == bag.BagId);
			if (!(bagSO == null))
			{
				AddBagToBackpack(bagSO, bag.CellIds, bag.ItemRotation);
			}
		}
		foreach (WeaponInBackpack weapon in backpackSaveState.WeaponsInBackpack)
		{
			WeaponSO weaponSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableWeapons.FirstOrDefault((WeaponSO w) => w.Id == weapon.WeaponId);
			if (!(weaponSO == null))
			{
				AddWeaponToBackpack(weaponSO, weapon.CellIds, weapon.ItemRotation);
			}
		}
		foreach (ItemInBackpack item in backpackSaveState.ItemsInBackpack)
		{
			ItemSO itemSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableItems.FirstOrDefault((ItemSO w) => w.Id == item.ItemId);
			if (!(itemSO == null))
			{
				AddItemToBackpack(itemSO, item.CellIds, item.ItemRotation);
			}
		}
	}

	internal BackpackSaveState GetSaveState()
	{
		BackpackSaveState backpackSaveState = new BackpackSaveState();
		foreach (BagInstance item in BackpackStorage.GetBagsInBackpack())
		{
			int id = item.Id;
			List<int> cellsPlaceableIsIn = BackpackStorage.GetCellsPlaceableIsIn(item);
			Enums.Backpack.ItemRotation currentRotation = item.BaseDraggable.CurrentRotation;
			backpackSaveState.AddBag(id, cellsPlaceableIsIn, currentRotation);
		}
		foreach (WeaponInstance item2 in BackpackStorage.GetWeaponsInBackpack())
		{
			int id2 = item2.Id;
			List<int> cellsPlaceableIsIn2 = BackpackStorage.GetCellsPlaceableIsIn(item2);
			Enums.Backpack.ItemRotation currentRotation2 = item2.BaseDraggable.CurrentRotation;
			backpackSaveState.AddWeapon(id2, cellsPlaceableIsIn2, currentRotation2);
		}
		foreach (ItemInstance item3 in BackpackStorage.GetItemsInBackpack())
		{
			int id3 = item3.Id;
			List<int> cellsPlaceableIsIn3 = BackpackStorage.GetCellsPlaceableIsIn(item3);
			Enums.Backpack.ItemRotation currentRotation3 = item3.BaseDraggable.CurrentRotation;
			backpackSaveState.AddItem(id3, cellsPlaceableIsIn3, currentRotation3);
		}
		return backpackSaveState;
	}

	public void SetDPSLoggableActiveStartTime()
	{
		List<IDPSLoggagable> allDpsLoggables = GetAllDpsLoggables();
		float time = Time.time;
		foreach (IDPSLoggagable item in allDpsLoggables)
		{
			item.ActiveSinceTime = time;
		}
	}

	public void LogDPSLoggableActiveTime()
	{
		List<IDPSLoggagable> allDpsLoggables = GetAllDpsLoggables();
		float time = Time.time;
		foreach (IDPSLoggagable item in allDpsLoggables)
		{
			float timeActive = time - item.ActiveSinceTime;
			SingletonController<WeaponDamageAndDPSController>.Instance.AddTimeActive(item.WeaponSO, timeActive);
		}
	}

	private List<IDPSLoggagable> GetAllDpsLoggables()
	{
		List<IDPSLoggagable> list = new List<IDPSLoggagable>();
		foreach (WeaponInstance item2 in GetWeaponsFromBackpack())
		{
			if (item2 != null)
			{
				list.Add(item2);
			}
		}
		foreach (BackpackSurvivors.Game.Relic.Relic activeRelic in SingletonController<RelicsController>.Instance.ActiveRelics)
		{
			if (activeRelic.RelicHandler is IDPSLoggagable item)
			{
				list.Add(item);
			}
		}
		return list;
	}
}
