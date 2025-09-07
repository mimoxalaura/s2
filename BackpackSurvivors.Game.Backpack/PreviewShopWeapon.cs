using BackpackSurvivors.Game.Items;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class PreviewShopWeapon : MonoBehaviour
{
	[SerializeField]
	private WeaponTooltipTrigger _weaponTooltipTrigger;

	[SerializeField]
	private Image _image;

	internal void Init(WeaponSO weaponSO)
	{
		SetImage(weaponSO.BackpackImage);
		WeaponInstance weaponInstance = new WeaponInstance(weaponSO);
		_weaponTooltipTrigger.SetWeaponContent(weaponInstance, Enums.Backpack.DraggableOwner.Shop);
	}

	internal void SetImage(Sprite sprite)
	{
		_image.sprite = sprite;
	}
}
