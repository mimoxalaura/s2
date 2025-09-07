using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class StatTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private Enums.ItemStatType _statType;

	private List<ItemStatModifier> _itemStatModifiers;

	private Enums.DamageType _damageType;

	private List<DamageTypeValueModifier> _damageTypeModifiers;

	private bool isStatType = true;

	private bool _active;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Stat;
	}

	public void SetStat(Enums.ItemStatType statType, List<ItemStatModifier> itemStatModifiers, bool active)
	{
		isStatType = true;
		_statType = statType;
		_itemStatModifiers = itemStatModifiers;
		_active = active;
	}

	public void SetStat(Enums.DamageType damageType, List<DamageTypeValueModifier> damageTypeValueModifiers, bool active)
	{
		isStatType = false;
		_damageType = damageType;
		_damageTypeModifiers = damageTypeValueModifiers;
		_active = active;
	}

	public override void ShowTooltip()
	{
		if (isStatType)
		{
			if (_instant)
			{
				SingletonController<TooltipController>.Instance.ShowStat(_statType, _itemStatModifiers, _active, this);
				return;
			}
			LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SingletonController<TooltipController>.Instance.ShowStat(_statType, _itemStatModifiers, _active, this);
			});
			_delayTweenId = lTDescr.uniqueId;
		}
		else if (_instant)
		{
			SingletonController<TooltipController>.Instance.ShowStat(_damageType, _damageTypeModifiers, _active, this);
		}
		else
		{
			LTDescr lTDescr2 = LeanTween.delayedCall(0.5f, (Action)delegate
			{
				SingletonController<TooltipController>.Instance.ShowStat(_damageType, _damageTypeModifiers, _active, this);
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
