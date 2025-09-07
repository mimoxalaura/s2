using System;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class IndexDetailPage : DetailPage
{
	[SerializeField]
	private Image _progressBarImage;

	[SerializeField]
	private TextMeshProUGUI _progressText;

	[SerializeField]
	private TextMeshProUGUI _progressPercentageText;

	[SerializeField]
	private Sprite _progressFull;

	[SerializeField]
	private Sprite _progressNotFull;

	internal void InitDetailPage()
	{
		int totalAvailableUnlocks = SingletonController<CollectionController>.Instance.GetTotalAvailableUnlocks();
		int totalUnlockedCount = SingletonController<SaveGameController>.Instance.ActiveSaveGame.CollectionsSaveState.GetTotalUnlockedCount();
		float num = (float)totalUnlockedCount / (float)totalAvailableUnlocks;
		decimal num2 = Math.Round((decimal)(num * 100f), 0, MidpointRounding.AwayFromZero);
		_progressPercentageText.SetText($"{num2}%");
		_progressText.SetText($"{totalUnlockedCount}/{totalAvailableUnlocks}");
		_progressBarImage.fillAmount = num;
		if (totalUnlockedCount >= totalAvailableUnlocks)
		{
			_progressBarImage.sprite = _progressFull;
		}
		else
		{
			_progressBarImage.sprite = _progressNotFull;
		}
	}
}
