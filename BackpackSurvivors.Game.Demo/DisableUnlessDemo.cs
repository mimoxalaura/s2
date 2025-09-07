using BackpackSurvivors.Game.Core;
using UnityEngine;

namespace BackpackSurvivors.Game.Demo;

internal class DisableUnlessDemo : MonoBehaviour
{
	private void Start()
	{
		if (!GameDatabase.IsDemo)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
