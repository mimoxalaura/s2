using System;

namespace BackpackSurvivors.Game.Characters.Events;

public class ExperienceGainedEventArgs : EventArgs
{
	public float CurrentExp { get; }

	public float MaxExpTillLevel { get; }

	public int CurrentLevel { get; }

	public float ExperienceGained { get; }

	public ExperienceGainedEventArgs(float currentExp, float maxExpTillLevel, int currentLevel, float experienceGained)
	{
		CurrentExp = currentExp;
		MaxExpTillLevel = maxExpTillLevel;
		CurrentLevel = currentLevel;
		ExperienceGained = experienceGained;
	}
}
