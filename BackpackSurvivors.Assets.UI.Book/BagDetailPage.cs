using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class BagDetailPage : DetailPage
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _tags;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _gameplayDescription;

	[SerializeField]
	private Image _itemBackground;

	internal void InitDetailPage(BagSO bag)
	{
		_image.sprite = bag.BackpackImage;
		_image.material = MaterialHelper.GetItemBorderRarityMaterial(bag.ItemRarity);
		_itemBackground.material = MaterialHelper.GetItemBorderRarityMaterial(bag.ItemRarity);
		_title.SetText(bag.Name ?? "");
		_description.SetText(bag.Description);
		_gameplayDescription.SetText("PLACEHOLDER - should be a detailed set of information similar to an expanded tooltip");
		_itemBackground.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(bag.ItemRarity);
	}
}
