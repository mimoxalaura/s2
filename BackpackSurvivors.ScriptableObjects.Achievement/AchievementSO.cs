using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Achievement;

[CreateAssetMenu(fileName = "Achievement", menuName = "Game/Achievement", order = 2)]
public class AchievementSO : ScriptableObject
{
	[SerializeField]
	public Enums.AchievementEnum AchievementEnum;

	[SerializeField]
	public string Name;

	[SerializeField]
	public Enums.SteamStat SteamStat;

	[SerializeField]
	public bool IsDemoAchievement;

	private void RenameFile()
	{
	}
}
