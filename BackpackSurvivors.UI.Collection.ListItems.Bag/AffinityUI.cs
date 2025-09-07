using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection.ListItems.Bag;

public class AffinityUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private DefaultTooltipTrigger _tooltip;

	private Enums.DamageType _damageType;

	private Enums.WeaponType _weaponType;

	private Enums.PlaceableWeaponSubtype _placeableWeaponSubtype;

	private bool _unlocked;

	public void Init(Enums.DamageType damageType)
	{
		Init(interactable: false);
		_damageType = damageType;
		_tooltip.SetDefaultContent($"{damageType}", $"This character finds substantial more {damageType} items", active: true);
		_image.sprite = SpriteHelper.GetDamageTypeIconSprite(damageType);
	}

	public void Init(Enums.WeaponType weaponType)
	{
		Init(interactable: false);
		_weaponType = weaponType;
		_tooltip.SetDefaultContent($"{weaponType}", $"This character finds substantial more {weaponType} items", active: true);
		_image.sprite = SpriteHelper.GetWeaponTypeIconSprite(weaponType);
	}

	public void Init(Enums.PlaceableWeaponSubtype placeableWeaponSubtype)
	{
		Init(interactable: false);
		_placeableWeaponSubtype = placeableWeaponSubtype;
		_tooltip.SetDefaultContent($"{placeableWeaponSubtype}", $"This character finds substantial more {placeableWeaponSubtype}s", active: true);
		_image.sprite = SpriteHelper.GetPlaceableWeaponSubtypeIconSprite(placeableWeaponSubtype);
	}

	private void Init(bool interactable)
	{
		GetComponent<MainButton>().ToggleHoverEffects(interactable);
		GetComponent<Button>().enabled = interactable;
	}
}
