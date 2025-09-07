using System;
using System.Collections.Generic;
using System.Linq;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class CollectionsSaveState : BaseSaveState
{
	public List<int> CollectedItemIds = new List<int>();

	public List<int> CollectedWeaponIds = new List<int>();

	public List<int> CollectedRelicIds = new List<int>();

	public List<int> CollectedCharacterIds = new List<int>();

	public List<int> CollectedEnemyIds = new List<int>();

	public List<int> CollectedBagIds = new List<int>();

	public List<int> FoundRecipes = new List<int>();

	public void SetState(List<int> collectedItemIds, List<int> collectedWeaponIds, List<int> collectedRelicIds, List<int> collectedCharacterIds, List<int> collectedEnemyIds, List<int> collectedBagIds, List<int> foundRecipes)
	{
		CollectedItemIds = ((collectedItemIds != null) ? collectedItemIds : new List<int>());
		CollectedWeaponIds = ((collectedWeaponIds != null) ? collectedWeaponIds : new List<int>());
		CollectedRelicIds = ((collectedRelicIds != null) ? collectedRelicIds : new List<int>());
		CollectedCharacterIds = ((collectedCharacterIds != null) ? collectedCharacterIds : new List<int>());
		CollectedEnemyIds = ((collectedEnemyIds != null) ? collectedEnemyIds : new List<int>());
		CollectedBagIds = ((collectedBagIds != null) ? collectedBagIds : new List<int>());
		FoundRecipes = ((foundRecipes != null) ? foundRecipes : new List<int>());
	}

	public override bool HasData()
	{
		if ((CollectedItemIds == null || !CollectedItemIds.Any()) && (CollectedWeaponIds == null || !CollectedWeaponIds.Any()) && (CollectedRelicIds == null || !CollectedRelicIds.Any()) && (CollectedCharacterIds == null || !CollectedCharacterIds.Any()) && (CollectedEnemyIds == null || !CollectedEnemyIds.Any()) && (CollectedBagIds == null || !CollectedBagIds.Any()))
		{
			if (FoundRecipes != null)
			{
				return FoundRecipes.Any();
			}
			return false;
		}
		return true;
	}

	internal int GetTotalUnlockedCount()
	{
		return CollectedItemIds.Count() + CollectedWeaponIds.Count() + CollectedRelicIds.Count() + CollectedEnemyIds.Count() + CollectedBagIds.Count() + FoundRecipes.Count();
	}
}
