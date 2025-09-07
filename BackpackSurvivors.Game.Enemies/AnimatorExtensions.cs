using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public static class AnimatorExtensions
{
	public static bool HasParameter(this Animator animator, string parameterName)
	{
		AnimatorControllerParameter[] parameters = animator.parameters;
		for (int i = 0; i < parameters.Length; i++)
		{
			if (parameters[i].name == parameterName)
			{
				return true;
			}
		}
		return false;
	}
}
