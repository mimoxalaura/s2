using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Shop;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.DEBUG.Backpack;

public class DEBUG_BackpackAutoLoader : MonoBehaviour
{
	public void Start()
	{
		StartCoroutine(DoStuff());
	}

	private IEnumerator DoStuff()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		Object.FindObjectOfType<ShopController>().GuaranteeItemInShop(31);
		Object.FindObjectOfType<ShopController>().GuaranteeWeaponInShop(2);
		Object.FindObjectOfType<ShopController>().GuaranteeBagInShop(5);
		SingletonController<CurrencyController>.Instance.GainCurrency(Enums.CurrencyType.Coins, 1000, Enums.CurrencySource.Drop);
		Object.FindObjectOfType<ShopController>().RerollShop();
	}
}
