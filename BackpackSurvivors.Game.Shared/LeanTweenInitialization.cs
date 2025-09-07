using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

internal class LeanTweenInitialization : MonoBehaviour
{
	private void Awake()
	{
		LeanTween.init(LeanTweenHelper.MaximumTweens);
	}
}
