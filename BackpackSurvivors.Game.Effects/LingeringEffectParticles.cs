using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class LingeringEffectParticles : MonoBehaviour
{
	[SerializeField]
	private bool _started = true;

	[SerializeField]
	private bool _completed;

	private void Update()
	{
		if (_started)
		{
			_started = false;
		}
		if (_completed)
		{
			_completed = false;
			End();
		}
	}

	public void End()
	{
		Object.Destroy(base.gameObject, 0f);
	}
}
