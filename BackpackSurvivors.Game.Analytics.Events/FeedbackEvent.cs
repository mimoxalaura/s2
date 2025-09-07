using Unity.Services.Analytics;

namespace BackpackSurvivors.Game.Analytics.Events;

internal class FeedbackEvent : Event
{
	public string FeedbackText
	{
		set
		{
			SetParameter("feedbackText", value);
		}
	}

	public FeedbackEvent()
		: base("feedback")
	{
	}
}
