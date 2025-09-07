using System;
using UnityEngine;

namespace BackpackSurvivors.System.Steam;

internal class SteamAchievement
{
	internal string Name { get; private set; }

	internal bool Achieved { get; private set; }

	internal float PercentAchieved { get; private set; }

	internal DateTime UnlockTime { get; private set; }

	internal string LocalizedName { get; private set; }

	internal string LocalizedDescription { get; private set; }

	internal bool Hidden { get; private set; }

	internal int SteamIconHandle { get; private set; }

	internal Texture2D AchievementImageTexture { get; private set; }

	public SteamAchievement(string name, bool achieved, float percentAchieved, DateTime unlockTime, string localizedName, string localizedDescription, bool hidden, int achievementIconHandle)
	{
		Name = name;
		Achieved = achieved;
		PercentAchieved = percentAchieved;
		UnlockTime = unlockTime;
		LocalizedName = localizedName;
		LocalizedDescription = localizedDescription;
		Hidden = hidden;
		SteamIconHandle = achievementIconHandle;
	}
}
