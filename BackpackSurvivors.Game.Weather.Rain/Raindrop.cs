using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Weather.Rain;

internal class Raindrop : MonoBehaviour
{
	private float duration = 0.5f;

	public void Init(RaindropController callbackPoint)
	{
		StartCoroutine(StartHiding(callbackPoint));
	}

	private IEnumerator StartHiding(RaindropController callbackPoint)
	{
		yield return new WaitForSeconds(duration);
		if (callbackPoint != null)
		{
			callbackPoint.RemovedDroplet();
		}
		base.gameObject.SetActive(value: false);
		Object.Destroy(base.gameObject);
	}
}
