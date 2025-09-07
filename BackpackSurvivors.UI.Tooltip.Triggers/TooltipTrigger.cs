using System;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[SerializeField]
	private bool _isEnabled;

	[SerializeField]
	internal string _header;

	[SerializeField]
	internal string _content;

	[SerializeField]
	internal bool _instant;

	[SerializeField]
	internal Enums.TooltipType TooltipType;

	protected int _delayTweenId;

	internal bool CanShowTooltip = true;

	public void SetContent(string headerToSet, string contentToSet)
	{
		_header = headerToSet;
		_content = contentToSet;
	}

	private void Start()
	{
		TooltipType = Enums.TooltipType.Default;
	}

	public virtual void UpdateContentVisual()
	{
		SingletonController<TooltipController>.Instance.SetBaseContent(_content, _header);
	}

	public void ToggleEnabled(bool enabled)
	{
		_isEnabled = enabled;
	}

	public virtual void ShowTooltip()
	{
		if (!CanShowTooltip || !_isEnabled)
		{
			return;
		}
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.Show(_content, this, _header);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.Show(_content, this, _header);
		});
		_delayTweenId = lTDescr.uniqueId;
	}

	public virtual void HideTooltip()
	{
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.Hide(null);
			return;
		}
		if (_delayTweenId != 0)
		{
			LeanTween.cancel(_delayTweenId);
			_delayTweenId = 0;
		}
		SingletonController<TooltipController>.Instance.Hide(null);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (SingletonController<InputController>.Instance.CursorEnabled)
		{
			ShowTooltip();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HideTooltip();
	}

	public void HideTooltipIfThisWasTarget(TooltipTrigger tooltipTriggerToClean)
	{
		SingletonController<TooltipController>.Instance.HideIfTriggerIsCorrect(tooltipTriggerToClean);
	}
}
