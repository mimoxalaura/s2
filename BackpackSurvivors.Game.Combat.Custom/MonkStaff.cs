using UnityEngine;

namespace BackpackSurvivors.Game.Combat.Custom;

internal class MonkStaff : MonoBehaviour
{
	private void Start()
	{
		LeanTween.rotate(base.gameObject, new Vector3(0f, 0f, 15000f), 10f).setDelay(0.2f);
	}

	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
