using System;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using UnityEngine;

namespace BackpackSurvivors.UI.Shop;

public class TrainingRoomItemSelectionUI : ModalUI
{
	[SerializeField]
	private Transform _itemParent;

	public void InitWeapons()
	{
		BaseDraggable prefabByItemType = GetPrefabByItemType(Enums.PlaceableType.Weapon);
		foreach (WeaponSO weapon in GameDatabaseHelper.GetWeapons())
		{
			BaseDraggable baseDraggable = UnityEngine.Object.Instantiate(prefabByItemType, _itemParent);
			InitDraggable(baseDraggable, weapon, Enums.PlaceableType.Weapon);
			baseDraggable.Enabled = true;
		}
	}

	public void InitItems()
	{
		foreach (ItemSO item in GameDatabaseHelper.GetItems())
		{
			BaseDraggable baseDraggable = UnityEngine.Object.Instantiate(GetPrefabByItemType(item.ItemType), _itemParent);
			InitDraggable(baseDraggable, item, item.ItemType);
			baseDraggable.Enabled = true;
		}
	}

	public void InitBags()
	{
		BaseDraggable prefabByItemType = GetPrefabByItemType(Enums.PlaceableType.Bag);
		foreach (BagSO bag in GameDatabaseHelper.GetBags())
		{
			BaseDraggable baseDraggable = UnityEngine.Object.Instantiate(prefabByItemType, _itemParent);
			InitDraggable(baseDraggable, bag, Enums.PlaceableType.Bag);
			baseDraggable.Enabled = true;
		}
	}

	public void ClearItems()
	{
		for (int num = _itemParent.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(_itemParent.GetChild(num).gameObject);
		}
	}

	private BaseDraggable GetPrefabByItemType(Enums.PlaceableType itemType)
	{
		return itemType switch
		{
			Enums.PlaceableType.Bag => GameDatabaseHelper.GetDraggableBagPrefab(), 
			Enums.PlaceableType.Weapon => GameDatabaseHelper.GetDraggableWeaponPrefab(), 
			Enums.PlaceableType.Item => GameDatabaseHelper.GetDraggableItemPrefab(), 
			_ => throw new Exception(string.Format("ItemType {0} is not handled in {1}.{2}()", itemType, "ShopOfferSlot", "GetPrefabByItemType")), 
		};
	}

	private void InitDraggable(BaseDraggable draggableGameObject, object sellableSO, Enums.PlaceableType itemType)
	{
		switch (itemType)
		{
		case Enums.PlaceableType.Bag:
			InitBag(draggableGameObject, sellableSO as BagSO);
			break;
		case Enums.PlaceableType.Weapon:
			InitWeapon(draggableGameObject, sellableSO as WeaponSO);
			break;
		case Enums.PlaceableType.Item:
			InitItem(draggableGameObject, sellableSO as ItemSO);
			break;
		default:
			throw new Exception(string.Format("ItemType {0} is not handled in {1}.{2}()", itemType, "ShopOfferSlot", "InitDraggable"));
		}
	}

	private void InitBag(BaseDraggable draggableGameObject, BagSO bagSO)
	{
		draggableGameObject.GetComponent<DraggableBag>().Init(bagSO, null, applySize: false);
		draggableGameObject.BaseItemInstance.SetBuyingPriceDiscount(1f);
	}

	private void InitWeapon(BaseDraggable draggableGameObject, WeaponSO weaponSO)
	{
		draggableGameObject.GetComponent<DraggableWeapon>().Init(weaponSO, null, applySize: false);
		draggableGameObject.BaseItemInstance.SetBuyingPriceDiscount(1f);
	}

	private void InitItem(BaseDraggable draggableGameObject, ItemSO itemSO)
	{
		draggableGameObject.GetComponent<DraggableItem>().Init(itemSO, null, applySize: false);
		draggableGameObject.BaseItemInstance.SetBuyingPriceDiscount(1f);
	}
}
