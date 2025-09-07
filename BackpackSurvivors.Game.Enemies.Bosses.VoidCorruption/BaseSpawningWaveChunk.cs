using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Bosses.VoidCorruption;

internal class BaseSpawningWaveChunk : MonoBehaviour
{
	internal virtual void Spawn()
	{
	}

	public virtual bool ShouldExecute()
	{
		return true;
	}
}
