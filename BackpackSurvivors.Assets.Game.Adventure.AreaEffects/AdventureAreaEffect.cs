using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Assets.Game.Adventure.AreaEffects;

internal class AdventureAreaEffect : MonoBehaviour
{
	internal virtual void Activate()
	{
		base.transform.position = SingletonController<GameController>.Instance.PlayerPosition;
	}
}
