using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.ScriptableObjects.Loot;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors;

public class LootDropContainer : MonoBehaviour
{
	internal static LootDropContainer Instance;

	private Dictionary<Enums.LootType, int> _dropCountInContainer = new Dictionary<Enums.LootType, int>();

	private List<Lootdrop> _lootInContainer;

	private int _maxNumberOfIdenticalLoot = 200;

	public bool ShouldSpawnNewEntity(Enums.LootType lootType)
	{
		return _dropCountInContainer[lootType] <= _maxNumberOfIdenticalLoot;
	}

	private void Awake()
	{
		SetupSingleton();
		_lootInContainer = new List<Lootdrop>();
		foreach (Enums.LootType value in Enum.GetValues(typeof(Enums.LootType)))
		{
			_dropCountInContainer.Add(value, 0);
		}
	}

	private void SetupSingleton()
	{
		if (Instance != null)
		{
			Debug.LogWarning("LootDropContainer already exists. Only one is supposed to exist per scene");
		}
		Instance = this;
	}

	internal void MoveLootdropsToPlayer(bool fromCompletion, List<Enums.LootType> lootTypesToMove, bool moveToUILayer = false)
	{
		IEnumerable<Lootdrop> enumerable = _lootInContainer.Where((Lootdrop ld) => lootTypesToMove.Contains(ld.LootType));
		if (lootTypesToMove.Contains(Enums.LootType.Coins))
		{
			CombineCoinPickupsIfNeeded(from ld in _lootInContainer
				where ld.LootType == Enums.LootType.Coins
				select ld as CoinPickup);
		}
		foreach (Lootdrop item in enumerable)
		{
			if (moveToUILayer)
			{
				item.SetLayer(SortingLayer.NameToID("UI"));
			}
			item.MoveLootdropToPlayer(fromCompletion);
		}
	}

	private void CombineCoinPickupsIfNeeded(IEnumerable<CoinPickup> coinPickups)
	{
		int num = 50;
		if (coinPickups.Count() < num)
		{
			return;
		}
		IOrderedEnumerable<CoinPickup> source = coinPickups.OrderBy((CoinPickup cp) => Vector2.Distance(cp.transform.position, SingletonController<GameController>.Instance.PlayerPosition));
		List<CoinPickup> list = source.Take(num).ToList();
		List<CoinPickup> list2 = source.Skip(num).ToList();
		int num2 = 0;
		foreach (CoinPickup item in list2)
		{
			CoinPickup targetCoin = list[num2 % num];
			CombineCoinPickups(item, targetCoin);
			RemoveDrop(Enums.LootType.Coins, item);
			UnityEngine.Object.Destroy(item.gameObject);
			num2++;
		}
	}

	private void CombineCoinPickups(CoinPickup sourceCoin, CoinPickup targetCoin)
	{
		targetCoin.AddValue(sourceCoin.CoinValue);
	}

	internal void AddDrop(Lootdrop lootdrop)
	{
		_lootInContainer.Add(lootdrop);
		AddDropCoin(lootdrop.LootType, 1);
	}

	private void AddDropCoin(Enums.LootType lootType, int amount)
	{
		_dropCountInContainer[lootType] += amount;
	}

	internal void RemoveDrop(Enums.LootType lootType, Lootdrop pickedUp)
	{
		_dropCountInContainer[lootType]--;
		_lootInContainer.Remove(pickedUp);
	}

	private float GenerateAmount(LootSO loot, float lootScaleFactor)
	{
		float num = UnityEngine.Random.Range(0f, 100f);
		if (loot.DropChance * SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.LuckPercentage) < num)
		{
			return 0f;
		}
		float num2 = 0f;
		if (loot.LootType == Enums.LootType.Coins)
		{
			num2 = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.ExtraCoinChancePercentage);
		}
		float num3 = (float)UnityEngine.Random.Range(loot.MinDropAmount, loot.MaxDropAmount) * (lootScaleFactor + num2);
		int num4 = (int)Math.Floor(num3);
		int num5 = (RandomHelper.GetRollSuccess(num3 - (float)num4) ? 1 : 0);
		return num4 + num5;
	}

	public float AddNewDrop(LootSO loot, Transform dropLocation, float lootScaleFactor)
	{
		if (ShouldSpawnNewEntity(loot.LootType))
		{
			return SpawnLootDrop(loot, dropLocation, lootScaleFactor);
		}
		AddValueToExistingLoot(loot);
		return 0f;
	}

	private void AddValueToExistingLoot(LootSO loot)
	{
		if (loot.LootType == Enums.LootType.Coins)
		{
			CoinPickup obj = (CoinPickup)GetFurthestAwayLootDrop(SingletonController<GameController>.Instance.Player.transform, loot.LootType);
			int value = UnityEngine.Random.Range(loot.MinDropAmount, loot.MaxDropAmount + 1);
			obj.AddValue(value);
		}
	}

	private float SpawnLootDrop(LootSO loot, Transform dropLocation, float lootScaleFactor)
	{
		float num = GenerateAmount(loot, lootScaleFactor);
		int num2 = (int)num;
		if (loot.LootType == Enums.LootType.Coins && num > 20f)
		{
			num = 1f;
		}
		for (int i = 0; (float)i < num; i++)
		{
			if (!(loot.Lootdrop == null))
			{
				CreateNewLootDrop(loot, dropLocation, num2);
			}
		}
		return num2;
	}

	private IEnumerator CreateNewLootDropAsync(LootSO loot, Transform dropLocation, int value)
	{
		Vector3 dropPosition = dropLocation.position;
		yield return new WaitForSeconds(0.05f);
		Lootdrop lootdrop = UnityEngine.Object.Instantiate(loot.Lootdrop, dropPosition, Quaternion.identity);
		if (loot.LootType == Enums.LootType.Coins)
		{
			((CoinPickup)lootdrop).UpdateValue(value);
		}
		lootdrop.transform.SetParent(base.transform);
		float num = 1.5f;
		float num2 = 1.5f;
		float x = UnityEngine.Random.Range(lootdrop.transform.localPosition.x - num, lootdrop.transform.localPosition.x + num);
		float y = UnityEngine.Random.Range(lootdrop.transform.localPosition.y - num2, lootdrop.transform.localPosition.y + num2);
		LeanTween.moveLocal(lootdrop.gameObject, new Vector3(x, y, 1f), 0.5f).setEaseOutBounce();
		AddDrop(lootdrop);
	}

	private void CreateNewLootDrop(LootSO loot, Transform dropLocation, int value)
	{
		StartCoroutine(CreateNewLootDropAsync(loot, dropLocation, value));
	}

	public Lootdrop GetFurthestAwayLootDrop(Transform playerTransform, Enums.LootType lootType)
	{
		IEnumerable<Lootdrop> enumerable = _lootInContainer.Where((Lootdrop x) => x.LootType == lootType);
		Lootdrop result = enumerable.FirstOrDefault();
		float num = 0f;
		foreach (Lootdrop item in enumerable)
		{
			float num2 = Vector3.Distance(item.transform.position, playerTransform.transform.position);
			if (num2 > num)
			{
				num = num2;
				result = item;
			}
		}
		return result;
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
