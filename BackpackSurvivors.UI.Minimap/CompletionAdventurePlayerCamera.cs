using BackpackSurvivors.Game.Player;
using UnityEngine;

namespace BackpackSurvivors.UI.Minimap;

public class CompletionAdventurePlayerCamera : MonoBehaviour
{
	private Transform _player;

	public void Init(Player player)
	{
		_player = player.transform;
	}

	private void LateUpdate()
	{
		if (_player != null)
		{
			Vector3 position = _player.position;
			position.z = base.transform.position.z;
			base.transform.position = position;
		}
	}
}
