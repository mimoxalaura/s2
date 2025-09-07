using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class RelicDetailPage : DetailPage
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

	internal void InitDetailPage(RelicSO relic)
	{
		_image.sprite = relic.Icon;
		_image.material = MaterialHelper.GetItemBorderRarityMaterial(relic.Rarity);
		_itemBackground.material = MaterialHelper.GetItemBorderRarityMaterial(relic.Rarity);
		_title.SetText(relic.Name ?? "");
		_description.SetText(relic.Description);
		_gameplayDescription.SetText("PLACEHOLDER - should be a detailed set of information similar to an expanded tooltip");
		_itemBackground.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(relic.Rarity);
	}
}
