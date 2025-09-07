using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BackpackSurvivors.Game.Shared;

[RequireComponent(typeof(Light2D))]
internal class FlickeringLight : MonoBehaviour
{
	[SerializeField]
	private bool _isActive;

	[SerializeField]
	private float _firstValue;

	[SerializeField]
	private float _secondValue;

	private float baseIntensity;

	private Light2D renderLight;

	private Queue<float> queue;

	public int smoothing = 5;

	private void Start()
	{
		renderLight = GetComponent<Light2D>();
		baseIntensity = renderLight.intensity;
		renderLight.intensity = 0f;
		queue = new Queue<float>();
		StartCoroutine(TimerLight());
	}

	public void ToggleActive(bool active)
	{
		_isActive = active;
		float intensity = (active ? baseIntensity : 0f);
		GetComponent<Light2D>().intensity = intensity;
	}

	private IEnumerator TimerLight()
	{
		float sum = 0f;
		while (true)
		{
			if (_isActive)
			{
				while (queue.Count > smoothing)
				{
					sum -= queue.Dequeue();
				}
				float num = Random.Range(_firstValue, _secondValue);
				queue.Enqueue(num);
				sum += num;
				renderLight.intensity = sum / (float)queue.Count;
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
