using System;
using System.Linq;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Collection;
using BackpackSurvivors.UI.Collection.ListItems.Bag;
using BackpackSurvivors.UI.Collection.ListItems.Item;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Level;

public class CharacterSwapDetailUINew : ModalUI
{
	[SerializeField]
	private TextMeshProUGUI _characterTitleText;

	[SerializeField]
	private Image _characterImage;

	[SerializeField]
	private TextMeshProUGUI _characterDescriptionText;

	[SerializeField]
	private Transform _startingGearContainer;

	[SerializeField]
	private Transform _affinityContainer;

	[SerializeField]
	private Transform _relicContainer;

	[SerializeField]
	private AffinityUI _affinityPrefab;

	[SerializeField]
	private CollectionItemUI _itemPrefab;

	[SerializeField]
	private CollectionBagUI _bagPrefab;

	[SerializeField]
	private CollectionWeaponUI _weaponPrefab;

	[SerializeField]
	private CollectionRelicUI _relicPrefab;

	public void Init(CharacterSO characterSO)
	{
		_characterTitleText.SetText(characterSO.Name);
		_characterImage.sprite = characterSO.Icon;
		_characterDescriptionText.SetText(characterSO.Description);
		DestroyChildren(_startingGearContainer);
		WeaponSO[] startingWeapons = characterSO.StartingWeapons;
		foreach (WeaponSO weapon in startingWeapons)
		{
			UnityEngine.Object.Instantiate(_weaponPrefab, _startingGearContainer).Init(weapon, unlocked: true, interactable: false);
		}
		ItemSO[] startingItems = characterSO.StartingItems;
		foreach (ItemSO item in startingItems)
		{
			UnityEngine.Object.Instantiate(_itemPrefab, _startingGearContainer).Init(item, unlocked: true, interactable: false);
		}
		BagSO[] startingBags = characterSO.StartingBags;
		foreach (BagSO bag in startingBags)
		{
			UnityEngine.Object.Instantiate(_bagPrefab, _startingGearContainer).Init(bag, unlocked: true, interactable: false);
		}
		RelicSO[] startingRelics = characterSO.StartingRelics;
		foreach (RelicSO relic in startingRelics)
		{
			UnityEngine.Object.Instantiate(_relicPrefab, _relicContainer).Init(relic, unlocked: true, interactable: false);
		}
		DestroyChildren(_affinityContainer);
		Enums.PlaceableTag combinedPlaceableTags = characterSO.CombinedPlaceableTags;
		_ = string.Empty;
		string[] names = Enum.GetNames(typeof(Enums.DamageType));
		string[] names2 = Enum.GetNames(typeof(Enums.PlaceableWeaponSubtype));
		string[] names3 = Enum.GetNames(typeof(Enums.WeaponType));
		foreach (Enum uniqueFlag in combinedPlaceableTags.GetUniqueFlags())
		{
			AffinityUI affinityUI = UnityEngine.Object.Instantiate(_affinityPrefab, _affinityContainer);
			if (names.Contains(uniqueFlag.ToString()))
			{
				Enums.DamageType damageType = (Enums.DamageType)Enum.Parse(typeof(Enums.DamageType), uniqueFlag.ToString());
				affinityUI.Init(damageType);
			}
			if (names2.Contains(uniqueFlag.ToString()))
			{
				Enums.PlaceableWeaponSubtype placeableWeaponSubtype = (Enums.PlaceableWeaponSubtype)Enum.Parse(typeof(Enums.PlaceableWeaponSubtype), uniqueFlag.ToString());
				affinityUI.Init(placeableWeaponSubtype);
			}
			if (names3.Contains(uniqueFlag.ToString()))
			{
				Enums.WeaponType weaponType = (Enums.WeaponType)Enum.Parse(typeof(Enums.WeaponType), uniqueFlag.ToString());
				affinityUI.Init(weaponType);
			}
		}
	}

	private void DestroyChildren(Transform parent)
	{
		for (int num = parent.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(parent.GetChild(num).gameObject);
		}
	}
}
