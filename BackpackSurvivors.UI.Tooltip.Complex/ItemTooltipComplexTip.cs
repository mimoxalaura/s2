using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip.Complex;

public class ItemTooltipComplexTip : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private Image _border;

	internal void Init(TipFeedbackElement tipFeedbackElement, Enums.PlaceableRarity rarity)
	{
		_title.SetText(tipFeedbackElement.Name);
		_description.SetText(tipFeedbackElement.Description);
		_icon.sprite = tipFeedbackElement.Icon;
		_border.material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(rarity);
	}
}
