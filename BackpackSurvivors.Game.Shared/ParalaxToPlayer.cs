using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Shared;

public class ParalaxToPlayer : MonoBehaviour
{
	[SerializeField]
	private float start = -32f;

	[SerializeField]
	private float end = 32f;

	[SerializeField]
	private float startMinimized = 5f;

	[SerializeField]
	private bool includeY;

	[SerializeField]
	private GameObject[] layers;

	private void Update()
	{
		float num = Mathf.InverseLerp(start, end, SingletonController<GameController>.Instance.PlayerPosition.x) * startMinimized * 2f;
		float y = base.transform.position.y;
		if (includeY)
		{
			y = SingletonController<GameController>.Instance.PlayerPosition.y;
		}
		base.transform.position = new Vector3(SingletonController<GameController>.Instance.PlayerPosition.x + startMinimized / 2f - num, y, base.transform.position.z);
		for (int i = 0; i < layers.Length; i++)
		{
			layers[i].transform.position = new Vector3(SingletonController<GameController>.Instance.PlayerPosition.x + startMinimized / 2f - num, y, base.transform.position.z);
		}
	}
}
