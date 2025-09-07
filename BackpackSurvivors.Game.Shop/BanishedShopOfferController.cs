using System.Collections.Generic;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Shop;

internal class BanishedShopOfferController : SingletonController<BanishedShopOfferController>
{
	private List<BagSO> _banishedBags = new List<BagSO>();

	private List<ItemSO> _banishedItems = new List<ItemSO>();

	private List<WeaponSO> _banishedWeapons = new List<WeaponSO>();

	public List<BagSO> BanishedBags => _banishedBags;

	public List<ItemSO> BanishedItems => _banishedItems;

	public List<WeaponSO> BanishedWeapons => _banishedWeapons;

	private void Start()
	{
		base.IsInitialized = true;
	}

	public void BanishBag(BagSO bag)
	{
		_banishedBags.Add(bag);
	}

	public void BanishItem(ItemSO item)
	{
		_banishedItems.Add(item);
	}

	public void BanishWeapon(WeaponSO weapon)
	{
		_banishedWeapons.Add(weapon);
	}

	public override void Clear()
	{
		ClearAllBanisheds();
	}

	public override void ClearAdventure()
	{
		ClearAllBanisheds();
	}

	private void ClearAllBanisheds()
	{
		_banishedBags.Clear();
		_banishedItems.Clear();
		_banishedWeapons.Clear();
	}
}
