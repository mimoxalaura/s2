using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Steam;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Achievements;

internal class AchievementItemUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private TextMeshProUGUI _descriptionText;

	[SerializeField]
	private TextMeshProUGUI _progressText;

	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private Image _iconBackdropImage;

	[SerializeField]
	private Material _completedMaterial;

	internal void Init(SteamAchievement steamAchievement)
	{
		SetText(steamAchievement);
		SetProgress(steamAchievement);
		SetIcon(steamAchievement);
	}

	private void SetIcon(SteamAchievement steamAchievement)
	{
		_iconImage.sprite = GetSpriteFromTexture(steamAchievement.AchievementImageTexture);
		_iconBackdropImage.sprite = _iconImage.sprite;
		_iconImage.gameObject.SetActive(steamAchievement.Achieved || steamAchievement.PercentAchieved > 0f);
		_iconImage.fillAmount = steamAchievement.PercentAchieved;
		if (steamAchievement.Achieved)
		{
			_iconImage.material = _completedMaterial;
		}
	}

	private Sprite GetSpriteFromTexture(Texture texture)
	{
		return null;
	}

	private void SetText(SteamAchievement steamAchievement)
	{
		string text = "15FF00";
		if (steamAchievement.Achieved)
		{
			_titleText.SetText("<color=#" + text + ">" + steamAchievement.LocalizedName + "</color>");
		}
		else
		{
			_titleText.SetText(steamAchievement.LocalizedName);
		}
		string noSprite = StringHelper.NoSprite;
		string yesSprite = StringHelper.YesSprite;
		string tooltipPoint = StringHelper.TooltipPoint;
		if (steamAchievement.PercentAchieved > 0f)
		{
			_descriptionText.SetText(tooltipPoint + steamAchievement.LocalizedDescription);
		}
		else if (steamAchievement.Achieved)
		{
			_descriptionText.SetText(yesSprite + " " + steamAchievement.LocalizedDescription);
		}
		else
		{
			_descriptionText.SetText(noSprite + " " + steamAchievement.LocalizedDescription);
		}
	}

	private void SetProgress(SteamAchievement steamAchievement)
	{
		_progressText.SetText($"{steamAchievement.PercentAchieved}%");
		if (!(steamAchievement.PercentAchieved > 0f))
		{
			_ = steamAchievement.Achieved;
		}
	}
}
