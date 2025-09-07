using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.System.Helper;

public static class LeanTweenHelper
{
	public static int MaximumTweens = 2000;

	public static void ChangeTransparency(Image gameObject, float transparancy)
	{
		gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, transparancy);
	}

	public static void ChangeTransparency(TextMeshProUGUI gameObject, float transparancy)
	{
		gameObject.color = new Color(gameObject.color.r, gameObject.color.g, gameObject.color.b, transparancy);
	}

	public static bool IsAtMaxCapacity()
	{
		bool num = LeanTween.tweensRunning >= MaximumTweens;
		if (num)
		{
			Debug.LogWarning("Above max lean tweens");
		}
		return num;
	}
}
