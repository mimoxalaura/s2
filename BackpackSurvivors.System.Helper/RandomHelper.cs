using UnityEngine;

namespace BackpackSurvivors.System.Helper;

internal class RandomHelper
{
	internal static bool GetRollSuccess(float chanceOfSuccess)
	{
		return Random.Range(0f, 1f) <= chanceOfSuccess;
	}

	internal static int GetRandomRoll(int maxCount)
	{
		return Random.Range(0, maxCount);
	}

	internal static bool GetRandomTrueFalse()
	{
		return Random.Range(0, 2) == 0;
	}
}
