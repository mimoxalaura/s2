using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionItemDetailUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _gameplayDescription;

	[SerializeField]
	private Image _itemBackground;

	public void ShowDetails(ItemSO item)
	{
		_image.sprite = item.BackpackImage;
		_title.SetText("<color=#" + ColorHelper.GetColorHexcodeForRarity(item.ItemRarity) + ">" + item.Name + "</color>");
		_description.SetText(item.Description);
		_gameplayDescription.SetText("PLACEHOLDER - should be a detailed set of information similar to an expanded tooltip");
		_itemBackground.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(item.ItemRarity);
	}
}
