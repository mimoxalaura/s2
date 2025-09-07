using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class MinimapCamera : MonoBehaviour
{
	[SerializeField]
	public Transform Player;

	private void Start()
	{
		Player = SingletonController<GameController>.Instance.Player.transform;
	}

	private void LateUpdate()
	{
		if (Player != null)
		{
			Vector3 position = Player.position;
			position.z = base.transform.position.z;
			base.transform.position = position;
		}
	}
}
