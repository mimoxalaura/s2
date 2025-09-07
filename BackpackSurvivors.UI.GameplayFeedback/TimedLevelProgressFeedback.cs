using System;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Level.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

internal class TimedLevelProgressFeedback : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _levelTimerText;

	[SerializeField]
	private Image _progressbarImage;

	internal void Init(int totalLevelDuration)
	{
		RegisterEvents();
		SetLevelTimerText(totalLevelDuration);
		base.gameObject.SetActive(value: true);
	}

	private void RegisterEvents()
	{
		UnityEngine.Object.FindObjectOfType<TimeBasedLevelController>().OnTimeRemainingInLevelUpdated += TimeBasedLevelController_OnTimeRemainingInLevelUpdated;
	}

	private void TimeBasedLevelController_OnTimeRemainingInLevelUpdated(object sender, TimeRemainingEventArgs e)
	{
		SetLevelTimerText(e.TimeRemaining);
		SetProgressBarFillPercentage(e.TimeRemaining, e.TotalLevelDuration);
	}

	private void SetProgressBarFillPercentage(int timeRemaining, int totalLevelDuration)
	{
		float fillAmount = (float)(totalLevelDuration - timeRemaining) / (float)totalLevelDuration;
		_progressbarImage.fillAmount = fillAmount;
	}

	private void SetLevelTimerText(int seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		_levelTimerText.text = $"{timeSpan.Minutes}:{timeSpan.Seconds:d2}";
	}
}
