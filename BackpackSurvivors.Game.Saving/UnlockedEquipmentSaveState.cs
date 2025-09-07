using System;
using System.Collections.Generic;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
internal class UnlockedEquipmentSaveState : BaseSaveState
{
	internal List<int> GenericUnlockedWeaponIds;

	internal List<int> GenericUnlockedItemIds;

	internal List<int> GenericUnlockedBagIds;

	internal List<int> GenericUnlockedRelicIds;

	internal Dictionary<int, int> CharacterSpecificUnlockedWeaponIds;

	internal Dictionary<int, int> CharacterSpecificUnlockedItemIds;

	internal Dictionary<int, int> CharacterSpecificUnlockedBagIds;

	internal Dictionary<int, int> CharacterSpecificUnlockedRelicIds;

	internal List<int> ReceivedRewardIds;

	public UnlockedEquipmentSaveState()
	{
		Init();
	}

	public override bool HasData()
	{
		bool flag = GenericUnlockedWeaponIds != null && GenericUnlockedWeaponIds.Count > 0;
		bool flag2 = CharacterSpecificUnlockedWeaponIds != null && CharacterSpecificUnlockedWeaponIds.Count > 0;
		bool flag3 = GenericUnlockedItemIds != null && GenericUnlockedItemIds.Count > 0;
		bool flag4 = CharacterSpecificUnlockedItemIds != null && CharacterSpecificUnlockedItemIds.Count > 0;
		bool num = GenericUnlockedBagIds != null && GenericUnlockedBagIds.Count > 0;
		bool flag5 = CharacterSpecificUnlockedBagIds != null && CharacterSpecificUnlockedBagIds.Count > 0;
		bool flag6 = GenericUnlockedRelicIds != null && GenericUnlockedRelicIds.Count > 0;
		bool flag7 = CharacterSpecificUnlockedRelicIds != null && CharacterSpecificUnlockedRelicIds.Count > 0;
		bool flag8 = ReceivedRewardIds != null && ReceivedRewardIds.Count > 0;
		return num || flag5 || flag || flag2 || flag3 || flag4 || flag6 || flag7 || flag8;
	}

	internal void SetState(List<int> genericUnlockedWeaponIds, Dictionary<int, int> characterSpecificUnlockedWeaponIds, List<int> genericUnlockedItemIds, Dictionary<int, int> characterSpecificUnlockedItemIds, List<int> genericUnlockedRelicIds, Dictionary<int, int> characterSpecificUnlockedRelicIds, List<int> genericUnlockedBagIds, Dictionary<int, int> characterSpecificUnlockedBagIds, List<int> receivedRewardIds)
	{
		GenericUnlockedWeaponIds = genericUnlockedWeaponIds;
		GenericUnlockedItemIds = genericUnlockedItemIds;
		GenericUnlockedRelicIds = genericUnlockedRelicIds;
		GenericUnlockedBagIds = genericUnlockedBagIds;
		CharacterSpecificUnlockedWeaponIds = characterSpecificUnlockedWeaponIds;
		CharacterSpecificUnlockedItemIds = characterSpecificUnlockedItemIds;
		CharacterSpecificUnlockedRelicIds = characterSpecificUnlockedRelicIds;
		CharacterSpecificUnlockedBagIds = characterSpecificUnlockedBagIds;
		ReceivedRewardIds = receivedRewardIds;
	}

	private void Init()
	{
		GenericUnlockedWeaponIds = new List<int>();
		GenericUnlockedItemIds = new List<int>();
		GenericUnlockedRelicIds = new List<int>();
		GenericUnlockedBagIds = new List<int>();
		CharacterSpecificUnlockedWeaponIds = new Dictionary<int, int>();
		CharacterSpecificUnlockedItemIds = new Dictionary<int, int>();
		CharacterSpecificUnlockedRelicIds = new Dictionary<int, int>();
		CharacterSpecificUnlockedBagIds = new Dictionary<int, int>();
		ReceivedRewardIds = new List<int>();
	}
}
