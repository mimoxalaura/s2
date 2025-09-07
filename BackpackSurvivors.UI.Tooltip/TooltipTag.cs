using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class TooltipTag : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _tagText;

	[SerializeField]
	private Image _tagsBorder;

	internal void SetTag(Enums.PlaceableTag tag, Enums.PlaceableRarity rarity)
	{
		string spriteValueByPlaceableTag = StringHelper.GetSpriteValueByPlaceableTag(tag);
		string cleanString = StringHelper.GetCleanString(tag);
		string color = ColorHelper.GetColor(tag);
		string sourceText = "<sprite name=\"" + spriteValueByPlaceableTag + "\"> <color=" + color + ">" + cleanString + "</color>";
		_tagText.SetText(sourceText);
	}

	internal float PreferredWidth()
	{
		return _tagText.preferredWidth;
	}
}
