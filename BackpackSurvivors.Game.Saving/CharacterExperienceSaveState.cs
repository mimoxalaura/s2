using System;
using System.Collections.Generic;
using System.Linq;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class CharacterExperienceSaveState : BaseSaveState
{
	public Dictionary<int, int> CharacterLevels;

	public Dictionary<int, float> CharacterRemainingExperience;

	public int ActiveCharacterId;

	public CharacterExperienceSaveState()
	{
		Init();
	}

	public void Init()
	{
		CharacterLevels = new Dictionary<int, int>();
		CharacterRemainingExperience = new Dictionary<int, float>();
		ActiveCharacterId = 1;
	}

	public void SetState(int activeCharacterId, Dictionary<int, int> characterLevels, Dictionary<int, float> characterRemainingExperience)
	{
		ActiveCharacterId = activeCharacterId;
		if (characterLevels != null)
		{
			foreach (KeyValuePair<int, int> characterLevel in characterLevels)
			{
				CharacterLevels.Add(characterLevel.Key, characterLevel.Value);
			}
		}
		if (characterRemainingExperience == null)
		{
			return;
		}
		foreach (KeyValuePair<int, float> item in characterRemainingExperience)
		{
			CharacterRemainingExperience.Add(item.Key, item.Value);
		}
	}

	public int GetLevelByCharacterId(int characterId)
	{
		if (!CharacterLevels.ContainsKey(characterId))
		{
			return 0;
		}
		return CharacterLevels[characterId];
	}

	public float GetExperienceByCharacterId(int characterId)
	{
		if (!CharacterRemainingExperience.ContainsKey(characterId))
		{
			return 0f;
		}
		return CharacterRemainingExperience[characterId];
	}

	public override bool HasData()
	{
		if (CharacterLevels != null)
		{
			return CharacterLevels.Any();
		}
		return false;
	}
}
