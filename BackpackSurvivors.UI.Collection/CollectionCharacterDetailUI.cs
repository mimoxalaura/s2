using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.UI.Collection.ListItems.Bag;
using BackpackSurvivors.UI.Collection.ListItems.Item;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Collection;

public class CollectionCharacterDetailUI : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _flavor;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private Transform _startingWeaponsContainer;

	[SerializeField]
	private CollectionWeaponUI _startingWeaponPrefab;

	[SerializeField]
	private Transform _startingItemsContainer;

	[SerializeField]
	private CollectionItemUI _startingItemPrefab;

	[SerializeField]
	private Transform _BagAndRelicContainer;

	[SerializeField]
	private CollectionBagUI _startingBagPrefab;

	[SerializeField]
	private CollectionRelicUI _startingRelicPrefab;

	[SerializeField]
	private Transform _AffinitiesContainer;

	[SerializeField]
	private AffinityUI _affinityPrefab;

	public void ShowDetails(CharacterSO characterClass)
	{
		_image.sprite = characterClass.Icon;
		_title.SetText(characterClass.Name);
		_description.SetText(characterClass.Description);
		_flavor.SetText(characterClass.Flavor);
		DestroyChildren(_startingWeaponsContainer);
		WeaponSO[] startingWeapons = characterClass.StartingWeapons;
		foreach (WeaponSO weapon in startingWeapons)
		{
			Object.Instantiate(_startingWeaponPrefab, _startingWeaponsContainer).Init(weapon, unlocked: true, interactable: false);
		}
		DestroyChildren(_startingItemsContainer);
		ItemSO[] startingItems = characterClass.StartingItems;
		foreach (ItemSO item in startingItems)
		{
			Object.Instantiate(_startingItemPrefab, _startingItemsContainer).Init(item, unlocked: true, interactable: false);
		}
		DestroyChildren(_BagAndRelicContainer);
		BagSO[] startingBags = characterClass.StartingBags;
		foreach (BagSO bag in startingBags)
		{
			Object.Instantiate(_startingBagPrefab, _BagAndRelicContainer).Init(bag, unlocked: true, interactable: false);
		}
		RelicSO[] startingRelics = characterClass.StartingRelics;
		foreach (RelicSO relic in startingRelics)
		{
			Object.Instantiate(_startingRelicPrefab, _BagAndRelicContainer).Init(relic, unlocked: true, interactable: false);
		}
	}

	private void DestroyChildren(Transform parent)
	{
		for (int num = parent.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(parent.GetChild(num).gameObject);
		}
	}
}
