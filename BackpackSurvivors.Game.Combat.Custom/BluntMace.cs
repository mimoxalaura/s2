using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Custom;

internal class BluntMace : MonoBehaviour
{
	private void Start()
	{
		LeanTween.rotate(base.gameObject, new Vector3(0f, 0f, 9000f), 10f);
	}

	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
