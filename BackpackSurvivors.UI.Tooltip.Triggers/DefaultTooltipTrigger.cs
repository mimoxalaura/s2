using System;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.System;
using UnityEngine.EventSystems;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class DefaultTooltipTrigger : TooltipTrigger, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private string _headerToShow;

	private string _contentToShow;

	private bool _active;

	private void Start()
	{
		TooltipType = Enums.TooltipType.Default;
	}

	public void SetDefaultContent(string header, string content, bool active)
	{
		_active = active;
		_headerToShow = header;
		_contentToShow = content;
		_header = header;
		_content = content;
		SingletonController<TooltipController>.Instance.SetBaseContent(_content, _header);
	}

	public override void ShowTooltip()
	{
		DragController controllerByType = SingletonCacheController.Instance.GetControllerByType<DragController>();
		if (controllerByType != null && controllerByType.IsDragging)
		{
			return;
		}
		if (_instant)
		{
			SingletonController<TooltipController>.Instance.SetBaseContent(_content, _header);
			SingletonController<TooltipController>.Instance.Show(_contentToShow, this, _headerToShow);
			return;
		}
		LTDescr lTDescr = LeanTween.delayedCall(0.5f, (Action)delegate
		{
			SingletonController<TooltipController>.Instance.SetBaseContent(_content, _header);
			SingletonController<TooltipController>.Instance.Show(_contentToShow, this, _headerToShow);
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
			_delayTweenId = 0;
		}
		SingletonController<TooltipController>.Instance.Hide(null);
	}
}
