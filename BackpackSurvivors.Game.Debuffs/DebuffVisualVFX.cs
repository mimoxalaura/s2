using UnityEngine;

namespace BackpackSurvivors.Game.Debuffs;

public class DebuffVisualVFX : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] _particlesToScale;

	internal void ScaleVFX(float scale)
	{
		ParticleSystem[] particlesToScale = _particlesToScale;
		for (int i = 0; i < particlesToScale.Length; i++)
		{
			ParticleSystem.MainModule main = particlesToScale[i].main;
			main.startSizeMultiplier *= scale;
		}
	}
}
