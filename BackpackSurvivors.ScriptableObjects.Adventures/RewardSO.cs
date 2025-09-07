using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Adventures;

[CreateAssetMenu(fileName = "Reward", menuName = "Game/Adventures/Reward", order = 1)]
public class RewardSO : ScriptableObject
{
	[SerializeField]
	public int Id;

	[SerializeField]
	public Enums.RewardType CompletionRewardType;

	[SerializeField]
	public ScriptableObject CompletionReward;

	[SerializeField]
	public int Amount;

	[SerializeField]
	public string Description;

	[Tooltip("The character this reward is unlocked for. Leave empty to have a generic reward for all characters")]
	[SerializeField]
	public CharacterSO BoundToCharacter;

	[Tooltip("Set this to true if you want the player to be able to receive this reward multiple times")]
	[SerializeField]
	public bool Repeatable;

	[SerializeField]
	public int HellfireLevel;

	private void RenameFile()
	{
	}

	private string GetRewardName()
	{
		return CompletionRewardType switch
		{
			Enums.RewardType.Weapon => ((WeaponSO)CompletionReward).Name, 
			Enums.RewardType.Item => ((ItemSO)CompletionReward).Name, 
			Enums.RewardType.Relic => ((RelicSO)CompletionReward).Name, 
			Enums.RewardType.TitanicSouls => "Titan Souls", 
			Enums.RewardType.Bag => ((BagSO)CompletionReward).Name, 
			_ => string.Empty, 
		};
	}
}
