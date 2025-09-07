using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class WeaponDetailPage : DetailPage
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
	private Image _itemBackground;

	[SerializeField]
	private CleanWeaponTooltip _weaponTooltip;

	private int weaponId = -1;

	private void Start()
	{
		_weaponTooltip.gameObject.SetActive(value: false);
	}

	internal void InitDetailPage(WeaponSO weapon)
	{
		if (weaponId != weapon.Id)
		{
			weaponId = weapon.Id;
			_image.sprite = weapon.BackpackImage;
			_image.SetNativeSize();
			Color colorForRarity = ColorHelper.GetColorForRarity(weapon.ItemRarity);
			_border.color = new Color(colorForRarity.r, colorForRarity.g, colorForRarity.b, 0.15f);
			_description.SetText(weapon.Description);
			_gameplayDescription.SetText(string.Empty);
			_itemBackground.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(weapon.ItemRarity);
			_weaponTooltip.gameObject.SetActive(value: true);
			_weaponTooltip.SetWeapon(weapon, Enums.Backpack.DraggableOwner.Collection, -1);
		}
	}
}
