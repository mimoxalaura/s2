using BackpackSurvivors.Game.Core;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors._DEBUG;

internal class DEBUG_DebugDisabler : MonoBehaviour
{
	private void Start()
	{
		if (!SingletonController<GameDatabase>.Instance.GameDatabaseSO.AllowDebugging)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
