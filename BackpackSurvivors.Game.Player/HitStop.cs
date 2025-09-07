using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

public class HitStop : MonoBehaviour
{
	private bool waiting;

	public void Stop(float duration, float timeScale)
	{
		if (!waiting)
		{
			Time.timeScale = timeScale;
			StartCoroutine(Wait(duration));
		}
	}

	public void Stop(float duration)
	{
		Stop(duration, 0f);
	}

	private IEnumerator Wait(float duration)
	{
		waiting = true;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1f;
		waiting = false;
	}
}
