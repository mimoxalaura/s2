using BackpackSurvivors.System;

namespace BackpackSurvivors.UI.Tooltip.Triggers;

public class InlineTextTooltipTrigger : TooltipTrigger
{
	private TipFeedbackElement _tipFeedbackElement;

	public void SetInlineContent(TipFeedbackElement tipFeedbackElement)
	{
		_tipFeedbackElement = tipFeedbackElement;
	}

	public override void ShowTooltip()
	{
		SingletonController<TooltipController>.Instance.ShowInlineTooltip(_tipFeedbackElement, this);
	}
}
