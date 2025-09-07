using System;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class BagTooltipTrigger : TooltipTrigger
{
	private BagSO _bagSO;

	private bool _active;

	private int _overridenPrice = -1;

	private Enums.Backpack.DraggableOwner _owner;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Bag;
	}

	public void SetBag(BagSO BagSO, bool active, Enums.Backpack.DraggableOwner owner)
	{
		_overridenPrice = -1;
		_bagSO = BagSO;
		_active = active;
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

	public override void ShowTooltip()
	{
		if (!CanShowTooltip)
		{
			return;
		}
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.ShowBag(_bagSO, _active, this, _owner, _overridenPrice);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.ShowBag(_bagSO, _active, this, _owner, _overridenPrice);
		});
		_delayTweenId = lTDescr.uniqueId;
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
