using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Loot;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors;

public class LootBag : MonoBehaviour
{
	private LootBagSO _lootBagSO;

	public void Init(LootBagSO lootBagSO)
	{
		_lootBagSO = lootBagSO;
	}

	public float TryDrop(Transform dropLocation, float lootScaleFactor)
	{
		if (_lootBagSO == null)
		{
			return 0f;
		}
		LootDropContainer lootDropContainer = SingletonController<GameController>.Instance.LootDropContainer;
		if (lootDropContainer == null)
		{
			return 0f;
		}
		float num = 0f;
		foreach (LootSO lootSO in _lootBagSO.LootSOs)
		{
			if (!(lootSO == null))
			{
				num += lootDropContainer.AddNewDrop(lootSO, dropLocation, lootScaleFactor);
			}
		}
		return num;
	}
}
