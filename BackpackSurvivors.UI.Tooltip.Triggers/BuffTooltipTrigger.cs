using System;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.System;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class BuffTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private BuffSO _buffSO;

	private float _timeRemaining;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Relic;
	}

	public void SetBuff(BuffSO buffSO)
	{
		_buffSO = buffSO;
		_timeRemaining = _buffSO.TimeUntillFalloff;
	}

	public void SetRemainingTime(float timeRemaining)
	{
		_timeRemaining = timeRemaining;
		SingletonController<TooltipController>.Instance.UpdateBuff(_buffSO, _timeRemaining, this);
	}

	public override void ShowTooltip()
	{
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.ShowBuff(_buffSO, _timeRemaining, this);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.ShowBuff(_buffSO, _timeRemaining, this);
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
