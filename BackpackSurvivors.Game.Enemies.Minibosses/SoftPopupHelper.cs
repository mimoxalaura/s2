using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

public class SoftPopupHelper : MonoBehaviour
{
	[SerializeField]
	private Image _iconImage;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private TextMeshProUGUI _rewardDescriptionText;

	[SerializeField]
	private Image _rewardDescriptionIconImage;

	[SerializeField]
	private Animator _animator;

	public void ShowInformationUI(string message, Sprite sprite, string description, Enums.SoftPopupType softPopupType)
	{
		_iconImage.sprite = sprite;
		_text.SetText(message);
		_rewardDescriptionText.SetText(description);
		if (softPopupType == Enums.SoftPopupType.FirstKill)
		{
			_rewardDescriptionIconImage.sprite = SpriteHelper.GetSoftPopupTypeSprite(softPopupType);
		}
		_animator.SetTrigger("Show");
	}
}
