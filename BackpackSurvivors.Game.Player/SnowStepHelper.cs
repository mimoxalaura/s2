using System;
using System.Collections;
using System.Linq;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace BackpackSurvivors.Game.Player;

public class SnowStepHelper : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private Player _player;

	[SerializeField]
	private float _minimumDistance = 0.5f;

	[SerializeField]
	private float _rateOverTime = 2f;

	[SerializeField]
	private float _timePerCheck = 0.1f;

	[SerializeField]
	private float _audioPlayDistance = 0.5f;

	[SerializeField]
	private AudioClip[] _snowStepAudioClips;

	private Vector2 _previousPosition;

	private Vector2 _currentPosition;

	private float _lastAudioClipPlayed;

	private void Start()
	{
	}

	private void OnEnable()
	{
		StartCoroutine(RunAsync());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private IEnumerator RunAsync()
	{
		while (base.isActiveAndEnabled)
		{
			_currentPosition = _player.transform.position;
			yield return new WaitForSeconds(_timePerCheck);
			if (Vector2.Distance(_previousPosition, _currentPosition) > _minimumDistance)
			{
				float num = _currentPosition.AngleTo(_previousPosition);
				ParticleSystem.MainModule main = _particleSystem.main;
				main.startRotation = num * (MathF.PI / 180f);
				ParticleSystem.EmissionModule emission = _particleSystem.emission;
				emission.rateOverTime = _rateOverTime;
				_previousPosition = _currentPosition;
				_lastAudioClipPlayed += Time.deltaTime;
				if (_lastAudioClipPlayed > _audioPlayDistance)
				{
					int randomRoll = RandomHelper.GetRandomRoll(_snowStepAudioClips.Count());
					SingletonController<AudioController>.Instance.PlaySFXClip(_snowStepAudioClips[randomRoll], 0.5f);
					_lastAudioClipPlayed = 0f;
				}
			}
			else
			{
				ParticleSystem.EmissionModule emission2 = _particleSystem.emission;
				emission2.rateOverTime = 0f;
			}
		}
	}
}
