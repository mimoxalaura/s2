using System;
using BackpackSurvivors.Game.Talents;
using BackpackSurvivors.ScriptableObjects.Talents;
using BackpackSurvivors.System;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class TalentTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private TalentSO _talentSO;

	private bool _active;

	private TalentTreePoint _talentTreePoint;

	public void Init(TalentTreePoint talentTreePoint)
	{
		_talentTreePoint = talentTreePoint;
	}

	private void Start()
	{
		TooltipType = Enums.TooltipType.Talent;
	}

	public void SetTooltipContent(TalentSO talentSO, bool active)
	{
		_talentSO = talentSO;
		_active = active;
	}

	public override void ShowTooltip()
	{
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.ShowTalent(_talentSO, _active, _talentTreePoint.ShouldShowActivatedPart(), this);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.ShowTalent(_talentSO, _talentTreePoint.ShouldShowActivatedPart(), _active, this);
		});
		_delayTweenId = lTDescr.uniqueId;
	}

	public override void UpdateContentVisual()
	{
		SingletonController<TooltipController>.Instance.UpdateTalentContent(_talentSO, _talentTreePoint.ShouldShowActivatedPart(), _active);
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
