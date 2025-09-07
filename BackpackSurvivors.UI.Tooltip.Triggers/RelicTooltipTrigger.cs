using System;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class RelicTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private RelicSO _relicSO;

	private bool _active;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Relic;
	}

	public void SetRelic(RelicSO relicSO, bool active)
	{
		_relicSO = relicSO;
		_active = active;
	}

	public override void ShowTooltip()
	{
		if (!_active)
		{
			return;
		}
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.ShowRelic(_relicSO, _active, this);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.ShowRelic(_relicSO, _active, this);
		}).setIgnoreTimeScale(useUnScaledTime: true);
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
