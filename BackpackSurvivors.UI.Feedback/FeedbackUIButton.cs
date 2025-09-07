using System;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Feedback;

public class FeedbackUIButton : MonoBehaviour
{
	private void Start()
	{
		SingletonController<InputController>.Instance.OnSpecial1Handler += InputController_OnSpecial1Handler;
	}

	private void OnDestroy()
	{
		SingletonController<InputController>.Instance.OnSpecial1Handler -= InputController_OnSpecial1Handler;
	}

	private void InputController_OnSpecial1Handler(object sender, EventArgs e)
	{
		FeedbackUI feedbackUI = UnityEngine.Object.FindObjectOfType<FeedbackUI>();
		if (feedbackUI != null)
		{
			feedbackUI.OpenFeedbackUI();
		}
	}
}
