using System;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class ItemTooltipTrigger : TooltipTrigger
{
	[SerializeField]
	private ItemInstance _itemInstance;

	[SerializeField]
	private ItemSO _itemSO;

	private Enums.Backpack.DraggableOwner _owner;

	private int _overridenPrice = -1;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Item;
	}

	public void SetItemContent(ItemInstance itemInstance, Enums.Backpack.DraggableOwner owner)
	{
		_itemInstance = itemInstance;
		ChangeOwner(owner);
	}

	public void ChangeOwner(Enums.Backpack.DraggableOwner owner)
	{
		_owner = owner;
	}

	public void SetDiscountedPrice(int discountedPrice)
	{
		_overridenPrice = discountedPrice;
	}

	public void SetItemContent(ItemSO itemSO)
	{
		_itemSO = itemSO;
	}

	public override void ShowTooltip()
	{
		if (_itemSO != null)
		{
			if (_instant)
			{
				SingletonController<TooltipController>.Instance.ShowItem(_itemSO, this, _owner, _overridenPrice);
				return;
			}
			LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SingletonController<TooltipController>.Instance.ShowItem(_itemSO, this, _owner, _overridenPrice);
			});
			_delayTweenId = lTDescr.uniqueId;
		}
		else
		{
			if (_itemInstance == null)
			{
				return;
			}
			if (_instant)
			{
				SingletonController<TooltipController>.Instance.ShowItem(_itemInstance, this, _owner, _overridenPrice);
				return;
			}
			LTDescr lTDescr2 = LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SingletonController<TooltipController>.Instance.ShowItem(_itemInstance, this, _owner, _overridenPrice);
			});
			_delayTweenId = lTDescr2.uniqueId;
		}
	}

	public override void HideTooltip()
	{
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.Hide(null);
			return;
		}
		if (_delayTweenId != 0)
		{
			LeanTween.cancel(_delayTweenId);
		}
		SingletonController<TooltipController>.Instance.Hide(null);
	}
}
