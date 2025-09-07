namespace BackpackSurvivors.UI.Tooltip;

public class InlineTextTooltip : BaseTooltip
{
	public void SetInlineContent(TipFeedbackElement tipFeedbackElement)
	{
		SetText(tipFeedbackElement.Description, tipFeedbackElement.Name);
	}
}
