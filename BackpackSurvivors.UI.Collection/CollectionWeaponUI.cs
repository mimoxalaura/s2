using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Collection.ListItems;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionWeaponUI : CollectionListItemUI
{
	public delegate void OnClickHandler(object sender, WeaponCollectionSelectedEventArgs e);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _backdropImage;

	[SerializeField]
	private GameObject _lockedOverlay;

	[SerializeField]
	private WeaponTooltipTrigger _tooltip;

	private WeaponSO _weapon;

	private bool _unlocked;

	public event OnClickHandler OnClick;

	public void Init(WeaponSO weapon, bool unlocked, bool interactable)
	{
		Init(interactable);
		_unlocked = unlocked;
		_image.sprite = weapon.Icon;
		_backdropImage.sprite = SpriteHelper.GetRarityTypeBackgroundSprite(weapon.ItemRarity);
		_weapon = weapon;
		_lockedOverlay.SetActive(!unlocked);
		if (_tooltip.enabled && _unlocked)
		{
			_tooltip.SetWeaponContent(weapon, Enums.Backpack.DraggableOwner.Shop);
			_tooltip.ToggleEnabled(unlocked);
		}
	}

	public void Click()
	{
		if (_unlocked)
		{
			this.OnClick?.Invoke(this, new WeaponCollectionSelectedEventArgs(_weapon, this));
		}
	}
}
