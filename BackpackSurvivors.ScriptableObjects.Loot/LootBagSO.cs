using System.Collections.Generic;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Loot;

[CreateAssetMenu(fileName = "LootBagSO", menuName = "Game/Items/LootBag", order = 1)]
public class LootBagSO : ScriptableObject
{
	[SerializeField]
	public List<LootSO> LootSOs;
}
