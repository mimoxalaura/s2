using System;
using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class ProjectileVisualizationFollower : MonoBehaviour
{
	[SerializeField]
	private float _delayAfterProjectileDestruction;

	[SerializeField]
	private float _delayBeforeEnable;

	[SerializeField]
	private GameObject[] _enableAfterDelay;

	private ProjectileVisualization _projectileVisualization;

	public void Init(ProjectileVisualization projectileVisualization)
	{
		_projectileVisualization = projectileVisualization;
		_projectileVisualization.OnDestroyEvent += _projectileVisualization_OnDestroyEvent;
		StartCoroutine(ShowAfterDelay());
	}

	private void _projectileVisualization_OnDestroyEvent(object sender, EventArgs e)
	{
		StartCoroutine(DestroyAfterDelay());
	}

	private IEnumerator DestroyAfterDelay()
	{
		yield return new WaitForSeconds(_delayAfterProjectileDestruction);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator ShowAfterDelay()
	{
		yield return new WaitForSeconds(_delayBeforeEnable);
		GameObject[] enableAfterDelay = _enableAfterDelay;
		for (int i = 0; i < enableAfterDelay.Length; i++)
		{
			enableAfterDelay[i].SetActive(value: true);
		}
	}

	private void Update()
	{
		if (!(_projectileVisualization == null))
		{
			base.transform.position = _projectileVisualization.transform.position;
		}
	}
}
