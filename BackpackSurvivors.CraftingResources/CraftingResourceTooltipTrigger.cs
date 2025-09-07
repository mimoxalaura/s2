using System;
using BackpackSurvivors.ScriptableObjects.CraftingResource;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip;
using BackpackSurvivors.UI.Tooltip.Triggers;

namespace BackpackSurvivors.CraftingResources;

internal class CraftingResourceTooltipTrigger : TooltipTrigger
{
	private CraftingResourceSO _craftingResourceSO;

	private int _amount;

	private void Start()
	{
		TooltipType = Enums.TooltipType.CraftingResource;
	}

	public void SetCraftingResource(CraftingResourceSO craftingResourceSO, int amount)
	{
		_craftingResourceSO = craftingResourceSO;
		_amount = amount;
	}

	public void UpdateAmount(int amount)
	{
		_amount = amount;
	}

	public override void ShowTooltip()
	{
		if (!CanShowTooltip)
		{
			return;
		}
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.ShowCraftingResource(_craftingResourceSO, this, _amount);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.ShowCraftingResource(_craftingResourceSO, this, _amount);
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
