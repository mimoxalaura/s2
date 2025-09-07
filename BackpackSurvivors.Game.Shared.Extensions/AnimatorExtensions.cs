using UnityEngine;

namespace BackpackSurvivors.Game.Shared.Extensions;

internal static class AnimatorExtensions
{
	internal static bool ParameterExists(this Animator source, string paramName)
	{
		if (source == null)
		{
			return false;
		}
		for (int i = 0; i < source.parameters.Length; i++)
		{
			if (source.parameters[i].name == paramName)
			{
				return true;
			}
		}
		return false;
	}
}
