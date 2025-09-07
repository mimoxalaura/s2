using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class ItemDetailPage : DetailPage
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _border;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _gameplayDescription;

	[SerializeField]
	private CleanItemTooltip _itemTooltip;

	private int itemId = -1;

	private void Start()
	{
		_itemTooltip.gameObject.SetActive(value: false);
	}

	internal void InitDetailPage(ItemSO item)
	{
		if (itemId != item.Id)
		{
			itemId = item.Id;
			_image.sprite = item.BackpackImage;
			_image.SetNativeSize();
			Color colorForRarity = ColorHelper.GetColorForRarity(item.ItemRarity);
			_border.color = new Color(colorForRarity.r, colorForRarity.g, colorForRarity.b, 0.15f);
			_description.SetText(item.Description);
			_gameplayDescription.SetText(string.Empty);
			_itemTooltip.gameObject.SetActive(value: true);
			_itemTooltip.SetItem(item, Enums.Backpack.DraggableOwner.Collection, -1);
		}
	}
}
