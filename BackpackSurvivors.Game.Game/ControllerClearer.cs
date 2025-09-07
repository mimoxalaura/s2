using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Game;

internal class ControllerClearer : MonoBehaviour
{
	[SerializeField]
	private bool _clearControllersOfAllState;

	[SerializeField]
	private bool _clearControllersOfAdventureState;

	private void Start()
	{
		if (_clearControllersOfAllState)
		{
			SingletonController<GameController>.Instance.ClearControllersOfAllState();
		}
		else if (_clearControllersOfAdventureState)
		{
			SingletonController<GameController>.Instance.ClearControllersOfAdventureState();
		}
	}
}
