using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

public class FollowPlayer : MonoBehaviour
{
	private void Update()
	{
		base.transform.position = new Vector3(SingletonController<GameController>.Instance.PlayerPosition.x, SingletonController<GameController>.Instance.PlayerPosition.y, base.transform.position.z);
	}
}
