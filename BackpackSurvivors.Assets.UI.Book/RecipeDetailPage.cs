using System.Linq;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Assets.UI.Book;

internal class RecipeDetailPage : DetailPage
{
	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private GameObject _selectARecipeText;

	[SerializeField]
	private GameObject _leftContainer;

	[SerializeField]
	private GameObject _rightContainer;

	[SerializeField]
	private GameObject _resultContainer;

	[SerializeField]
	private TextMeshProUGUI _itemLeftAmount;

	[SerializeField]
	private TextMeshProUGUI _itemLeftName;

	[SerializeField]
	private Image _itemLeftBorder;

	[SerializeField]
	private Image _itemLeft;

	[SerializeField]
	private ItemTooltipTrigger _itemTooltipTriggerLeft;

	[SerializeField]
	private WeaponTooltipTrigger _weaponTooltipTriggerLeft;

	[SerializeField]
	private GameObject _itemLeftBackpackIcon;

	[SerializeField]
	private GameObject _itemLeftWeaponIcon;

	[SerializeField]
	private GameObject _itemLeftItemIcon;

	[SerializeField]
	private TextMeshProUGUI _itemRightAmount;

	[SerializeField]
	private TextMeshProUGUI _itemRightName;

	[SerializeField]
	private Image _itemRightBorder;

	[SerializeField]
	private Image _itemRight;

	[SerializeField]
	private ItemTooltipTrigger _itemTooltipTriggerRight;

	[SerializeField]
	private WeaponTooltipTrigger _weaponTooltipTriggerRight;

	[SerializeField]
	private GameObject _itemRightBackpackIcon;

	[SerializeField]
	private GameObject _itemRightWeaponIcon;

	[SerializeField]
	private GameObject _itemRightItemIcon;

	[SerializeField]
	private Image _itemResultBorder;

	[SerializeField]
	private TextMeshProUGUI _itemResultName;

	[SerializeField]
	private Image _itemResult;

	[SerializeField]
	private ItemTooltipTrigger _itemTooltipTriggerResult;

	[SerializeField]
	private WeaponTooltipTrigger _weaponTooltipTriggerResult;

	[SerializeField]
	private GameObject _itemResultBackpackIcon;

	[SerializeField]
	private GameObject _itemResultWeaponIcon;

	[SerializeField]
	private GameObject _itemResultItemIcon;

	internal void InitDetailPage(MergableSO recipe)
	{
		_selectARecipeText.SetActive(value: false);
		_leftContainer.SetActive(value: true);
		_rightContainer.SetActive(value: true);
		_resultContainer.SetActive(value: true);
		if (recipe.Input.Count == 2)
		{
			SetItemBlockLeft(recipe.Input.FirstOrDefault((MergeableIngredient x) => x.IsPrimary));
			SetItemBlockRight(recipe.Input.FirstOrDefault((MergeableIngredient x) => !x.IsPrimary));
			SetItemResult(recipe.Output);
		}
	}

	private void SetItemResult(MergeableResult output)
	{
		_title.SetText(output.BaseItem.Name ?? "");
		_itemResultName.SetText(output.BaseItem.Name ?? "");
		_itemResultBorder.material = MaterialHelper.GetShopOfferBorderRarityMaterial(output.BaseItem.ItemRarity);
		_itemResult.sprite = output.BaseItem.BackpackImage;
		_itemResult.material = MaterialHelper.GetItemBorderRarityMaterial(output.BaseItem.ItemRarity);
		_itemResult.SetNativeSize();
		_itemTooltipTriggerResult.ChangeOwner(Enums.Backpack.DraggableOwner.Collection);
		_itemTooltipTriggerResult.ToggleEnabled(output.BaseItem.ItemType == Enums.PlaceableType.Item);
		_weaponTooltipTriggerResult.ToggleEnabled(output.BaseItem.ItemType == Enums.PlaceableType.Weapon);
		if (output.BaseItem.ItemType == Enums.PlaceableType.Item)
		{
			_itemTooltipTriggerResult.SetItemContent((ItemSO)output.BaseItem);
		}
		if (output.BaseItem.ItemType == Enums.PlaceableType.Weapon)
		{
			_weaponTooltipTriggerResult.SetWeaponContent((WeaponSO)output.BaseItem, Enums.Backpack.DraggableOwner.Collection);
		}
		_itemResultBackpackIcon.gameObject.SetActive(output.BaseItem.ItemType == Enums.PlaceableType.Bag);
		_itemResultItemIcon.gameObject.SetActive(output.BaseItem.ItemType == Enums.PlaceableType.Item);
		_itemResultWeaponIcon.gameObject.SetActive(output.BaseItem.ItemType == Enums.PlaceableType.Weapon);
	}

	private void SetItemBlockRight(MergeableIngredient mergeableIngredient)
	{
		_itemRightAmount.SetText($"{mergeableIngredient.Amount}x");
		_itemRightName.SetText(mergeableIngredient.BaseItem.Name ?? "");
		_itemRightBorder.material = MaterialHelper.GetShopOfferBorderRarityMaterial(mergeableIngredient.BaseItem.ItemRarity);
		_itemRight.sprite = mergeableIngredient.BaseItem.BackpackImage;
		_itemRight.material = MaterialHelper.GetItemBorderRarityMaterial(mergeableIngredient.BaseItem.ItemRarity);
		_itemRight.SetNativeSize();
		_itemTooltipTriggerRight.ChangeOwner(Enums.Backpack.DraggableOwner.Collection);
		_itemTooltipTriggerRight.ToggleEnabled(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Item);
		_weaponTooltipTriggerRight.ToggleEnabled(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Weapon);
		if (mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Item)
		{
			_itemTooltipTriggerRight.SetItemContent((ItemSO)mergeableIngredient.BaseItem);
		}
		if (mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Weapon)
		{
			_weaponTooltipTriggerRight.SetWeaponContent((WeaponSO)mergeableIngredient.BaseItem, Enums.Backpack.DraggableOwner.Collection);
		}
		_itemRightBackpackIcon.gameObject.SetActive(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Bag);
		_itemRightItemIcon.gameObject.SetActive(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Item);
		_itemRightWeaponIcon.gameObject.SetActive(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Weapon);
	}

	private void SetItemBlockLeft(MergeableIngredient mergeableIngredient)
	{
		_itemLeftAmount.SetText($"{mergeableIngredient.Amount}x");
		_itemLeftName.SetText(mergeableIngredient.BaseItem.Name ?? "");
		_itemLeftBorder.material = MaterialHelper.GetShopOfferBorderRarityMaterial(mergeableIngredient.BaseItem.ItemRarity);
		_itemLeft.sprite = mergeableIngredient.BaseItem.BackpackImage;
		_itemLeft.material = MaterialHelper.GetItemBorderRarityMaterial(mergeableIngredient.BaseItem.ItemRarity);
		_itemLeft.SetNativeSize();
		_itemTooltipTriggerLeft.ChangeOwner(Enums.Backpack.DraggableOwner.Collection);
		_itemTooltipTriggerLeft.ToggleEnabled(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Item);
		_weaponTooltipTriggerLeft.ToggleEnabled(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Weapon);
		if (mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Item)
		{
			_itemTooltipTriggerLeft.SetItemContent((ItemSO)mergeableIngredient.BaseItem);
		}
		if (mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Weapon)
		{
			_weaponTooltipTriggerLeft.SetWeaponContent((WeaponSO)mergeableIngredient.BaseItem, Enums.Backpack.DraggableOwner.Collection);
		}
		_itemLeftBackpackIcon.gameObject.SetActive(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Bag);
		_itemLeftItemIcon.gameObject.SetActive(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Item);
		_itemLeftWeaponIcon.gameObject.SetActive(mergeableIngredient.BaseItem.ItemType == Enums.PlaceableType.Weapon);
	}
}
