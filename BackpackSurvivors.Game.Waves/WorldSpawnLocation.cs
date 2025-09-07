using UnityEngine;

namespace BackpackSurvivors.Game.Waves;

internal class WorldSpawnLocation : MonoBehaviour
{
	public float MinX => base.transform.position.x - base.transform.localScale.x / 2f;

	public float MaxX => base.transform.position.x + base.transform.localScale.x / 2f;

	public float MinY => base.transform.position.y - base.transform.localScale.y / 2f;

	public float MaxY => base.transform.position.y + base.transform.localScale.y / 2f;
}
