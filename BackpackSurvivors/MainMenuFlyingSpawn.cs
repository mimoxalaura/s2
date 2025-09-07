using System.Collections;
using UnityEngine;

namespace BackpackSurvivors;

public class MainMenuFlyingSpawn : MonoBehaviour
{
	private Vector3 _startPosition;

	private bool _isFlying;

	[SerializeField]
	private float _flySpeed;

	[SerializeField]
	private float _flyChance;

	[SerializeField]
	private float _flyCheckTime;

	[SerializeField]
	private float _flyDuration;

	private void Start()
	{
		_startPosition = base.transform.position;
		StartCoroutine(TryStartFlying());
	}

	private void Update()
	{
		if (_isFlying)
		{
			base.transform.Translate(Vector3.right * Time.deltaTime * _flySpeed);
		}
	}

	private IEnumerator TryStartFlying()
	{
		while (true)
		{
			if (!_isFlying)
			{
				float num = Random.Range(0f, 1f);
				if (_flyChance > num)
				{
					_isFlying = true;
					base.transform.position = _startPosition;
					StartCoroutine(StartEndFlyingTimer());
				}
			}
			yield return new WaitForSecondsRealtime(_flyCheckTime);
		}
	}

	private IEnumerator StartEndFlyingTimer()
	{
		yield return new WaitForSecondsRealtime(_flyDuration);
		_isFlying = false;
	}
}
