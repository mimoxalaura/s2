using System.Linq;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Achievements;

internal class AchievementsUI : ModalUI
{
	[SerializeField]
	internal AchievementItemUI _achievementItemUIPrefab;

	[SerializeField]
	internal Transform _achievementListContainer;

	[SerializeField]
	internal Image _progressBarImage;

	[SerializeField]
	internal TextMeshProUGUI _progressText;

	private void Start()
	{
		for (int num = _achievementListContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(_achievementListContainer.GetChild(num).gameObject);
		}
	}

	internal void Init(IOrderedEnumerable<SteamAchievement> steamAchievements)
	{
		foreach (SteamAchievement steamAchievement in steamAchievements)
		{
			AddAchievement(steamAchievement);
		}
		int num = steamAchievements.Count((SteamAchievement x) => x.Achieved);
		int num2 = steamAchievements.Count();
		float num3 = (float)num / (float)num2;
		_progressBarImage.fillAmount = num3;
		int num4 = (int)(num3 * 100f);
		_progressText.SetText($"{num4}%");
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.CloseUI(openDirection);
	}

	public override void AfterOpenUI()
	{
		base.AfterOpenUI();
	}

	public override void AfterCloseUI()
	{
		base.AfterCloseUI();
	}

	internal void AddAchievement(SteamAchievement steamAchievement)
	{
		Object.Instantiate(_achievementItemUIPrefab, _achievementListContainer).Init(steamAchievement);
	}
}
