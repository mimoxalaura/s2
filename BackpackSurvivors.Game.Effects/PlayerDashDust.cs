using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

internal class PlayerDashDust : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(DestroyAfterDelay());
	}

	private IEnumerator DestroyAfterDelay()
	{
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(base.gameObject);
	}
}
