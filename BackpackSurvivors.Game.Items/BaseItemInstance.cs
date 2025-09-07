using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Items;

public class BaseItemInstance
{
	internal MergeRecipeSet CurrentMergeRecipeSet;

	private MergeToastNotification _activeToast;

	private float _buyingDiscount;

	private float _sellingDiscount;

	public BaseItemSO BaseItemSO { get; private set; }

	internal BaseDraggable BaseDraggable { get; private set; }

	public Guid Guid { get; private set; }

	public int Id => BaseItemSO.Id;

	public float DroppedInBackpackAtTime { get; private set; }

	public bool MergingAllowed { get; private set; } = true;

	internal List<Guid> PotentialMergeItems { get; private set; } = new List<Guid>();

	internal List<BaseItemInstance> BaseItemsThisCanMergeWith { get; private set; } = new List<BaseItemInstance>();

	public int TotalPlaceableSize => BaseItemSO.ItemSize.SizeInfo.Count((Enums.Backpack.ItemSizeCellType c) => c == Enums.Backpack.ItemSizeCellType.CellContainsPlacable);

	public int BuyingPrice => GetBuyingPrice();

	public int SellingPrice => GetSellingPrice();

	public void ClearPotentialMergeItems()
	{
		PotentialMergeItems.Clear();
	}

	internal void AddToPotentialMergeItems(List<Guid> items)
	{
		foreach (Guid item in items)
		{
			if (!PotentialMergeItems.Contains(item))
			{
				PotentialMergeItems.Add(item);
			}
		}
	}

	public void ClearMergePossibilities()
	{
		BaseItemsThisCanMergeWith.Clear();
	}

	public void AddItemsThisCanMergeWith(List<BaseItemInstance> items)
	{
		BaseItemsThisCanMergeWith.AddRange(items);
	}

	public void SetBaseItemInstance(BaseItemSO baseItemSO)
	{
		BaseItemSO = baseItemSO;
		Guid = Guid.NewGuid();
	}

	public void SetBuyingPriceDiscount(float discount)
	{
		_buyingDiscount = discount;
	}

	public void SetSellingPriceDiscount(float discount)
	{
		_sellingDiscount = discount;
	}

	public void UpdateDroppedInBackpackTime()
	{
		DroppedInBackpackAtTime = Time.unscaledTime;
	}

	public void ToggleMergingAllowed()
	{
		MergingAllowed = !MergingAllowed;
	}

	public void SetMergingAllowed(bool allowed)
	{
		MergingAllowed = allowed;
	}

	private int GetBuyingPrice()
	{
		return (int)((float)BaseItemSO.BuyingPrice - (float)BaseItemSO.BuyingPrice * _buyingDiscount);
	}

	private int GetSellingPrice()
	{
		return (int)((float)BaseItemSO.SellingPrice - (float)BaseItemSO.SellingPrice * _sellingDiscount);
	}

	internal void SetDraggable(BaseDraggable baseDraggable)
	{
		BaseDraggable = baseDraggable;
	}

	internal void SetActiveToast(MergeToastNotification toast)
	{
		_activeToast = toast;
	}

	internal void ClearActiveToast()
	{
		if (!(_activeToast == null))
		{
			UnityEngine.Object.Destroy(_activeToast.gameObject);
		}
	}
}
