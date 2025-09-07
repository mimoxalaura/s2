using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class PreviewShopItem : MonoBehaviour
{
	[SerializeField]
	private DefaultTooltipTrigger _defaultTooltipTrigger;

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _effectImage;

	internal void Init(BaseItemSO itemSO)
	{
		SetImage(itemSO.BackpackImage);
		string text = "<color=#" + ColorHelper.GetColorHexcodeForRarity(itemSO.ItemRarity) + ">" + itemSO.Name + "</color>";
		string text2 = "<color=#FFFFFF>Sneak preview !</color>\r\nComing in the full release";
		_defaultTooltipTrigger.SetContent(text, text2);
		_defaultTooltipTrigger.SetDefaultContent(text, text2, active: true);
	}

	internal void SetImage(Sprite sprite)
	{
		_image.sprite = sprite;
		_effectImage.sprite = sprite;
	}
}
