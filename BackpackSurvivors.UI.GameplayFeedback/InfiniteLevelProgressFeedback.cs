using System;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Level.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

internal class InfiniteLevelProgressFeedback : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _levelTimerText;

	[SerializeField]
	private Image _progressbarImage;

	[SerializeField]
	private GameObject _flameObject;

	[SerializeField]
	private TextMeshProUGUI _flameText;

	private string lastDifficulty = "1.0";

	private InfiniteLevelController _infiniteLevelController;

	internal void Init()
	{
		RegisterEvents();
		base.gameObject.SetActive(value: true);
		_levelTimerText.SetText("Infinity Arena [00:00:00]");
	}

	private void RegisterEvents()
	{
		_infiniteLevelController = UnityEngine.Object.FindObjectOfType<InfiniteLevelController>();
		_infiniteLevelController.OnTimeRemainingInLevelUpdated += LevelController_OnTimeRemainingInLevelUpdated;
	}

	private void SetProgressBarFillPercentage(double percentage)
	{
		_progressbarImage.fillAmount = (float)percentage;
	}

	private void LevelController_OnTimeRemainingInLevelUpdated(object sender, TimeRemainingEventArgs e)
	{
		double num = (float)_infiniteLevelController.TimeSpentInLevel / _infiniteLevelController.DifficultyScaler;
		double progressBarFillPercentage = num - Math.Truncate(num);
		SetProgressBarFillPercentage(progressBarFillPercentage);
		string text = ((float)_infiniteLevelController.TimeSpentInLevel / _infiniteLevelController.DifficultyScaler + 1f).ToString("F1");
		TimeSpan timeSpan = TimeSpan.FromSeconds(_infiniteLevelController.TimeSpentInLevel);
		if (timeSpan.Hours > 0)
		{
			_levelTimerText.SetText(string.Format("Infinity Arena [{0}:{1}:{2:d2}]", timeSpan.Hours.ToString("00"), timeSpan.Minutes.ToString("00"), timeSpan.Seconds));
		}
		else
		{
			_levelTimerText.SetText(string.Format("Infinity Arena [{0}:{1:d2}]", timeSpan.Minutes.ToString("00"), timeSpan.Seconds));
		}
		if (lastDifficulty != text)
		{
			lastDifficulty = text;
			AnimateDifficultyIncrease(text);
		}
	}

	private void AnimateDifficultyIncrease(string difficulty)
	{
		Debug.Log("Difficulty increased");
		LeanTween.scale(_flameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.3f);
		LeanTween.scale(_flameObject, new Vector3(1f, 1f, 1f), 0.5f).setEaseInOutElastic().setDelay(0.3f);
		_flameText.SetText(difficulty + "x");
	}
}
