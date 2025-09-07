using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.UI.Shared;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Light2D))]
internal class CursorFollower : MonoBehaviour
{
	public void ToggleParticles(bool showParticles)
	{
		if (showParticles)
		{
			GetComponent<ParticleSystem>().Play();
		}
		else
		{
			GetComponent<ParticleSystem>().Pause();
		}
	}

	internal void ToggleLight(bool enableLight)
	{
		GetComponent<Light2D>().enabled = enableLight;
	}

	private void Update()
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z - base.transform.position.z)));
		position.z = base.transform.position.z;
		base.transform.position = position;
	}
}
