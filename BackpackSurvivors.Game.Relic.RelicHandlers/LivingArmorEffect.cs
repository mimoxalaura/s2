using UnityEngine;

namespace BackpackSurvivors.Game.Relic.RelicHandlers;

public class LivingArmorEffect : MonoBehaviour
{
	private float _shieldScale = 0.6f;

	private void Start()
	{
		LeanTween.scale(base.gameObject, new Vector3(_shieldScale, _shieldScale, _shieldScale), 0.3f);
	}

	internal void RemoveEffect()
	{
		LeanTween.scale(base.gameObject, new Vector3(0f, 0f, 0f), 0.2f);
	}
}
