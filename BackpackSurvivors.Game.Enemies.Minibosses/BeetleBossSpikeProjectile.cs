using System.Collections;
using BackpackSurvivors.Game.Combat.Droppables;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

internal class BeetleBossSpikeProjectile : MonoBehaviour
{
	[SerializeField]
	private SpawnExplosionAfterWarning _spawnExplosionAfterWarningPrefab;

	[SerializeField]
	private float _minFlightTime;

	[SerializeField]
	private float _maxFlightTime;

	[SerializeField]
	private AudioClip _spikeLand;

	[SerializeField]
	private AudioClip _spikeFalling;

	[SerializeField]
	private AudioClip _spikeStart;

	[SerializeField]
	private float _outsideRangeYDistance;

	private Vector2 _targetPosition;

	private Vector2 _sourcePosition;

	private float _flightTime;

	private float _hellfireDamageMultiplier;

	private SpawnExplosionAfterWarning _spawnExplosionAfterWarning;

	public void StartFlying(Vector2 targetPosition, Vector2 sourcPositione, bool randomFlyTime = true)
	{
		_targetPosition = targetPosition;
		_sourcePosition = sourcPositione;
		if (randomFlyTime)
		{
			_flightTime = Random.Range(_minFlightTime, _maxFlightTime);
		}
		else
		{
			_flightTime = _minFlightTime;
		}
		StartCoroutine(ProjectileFlight());
		StartCoroutine(ShowWarning());
	}

	private IEnumerator ShowWarning()
	{
		float seconds = _flightTime - _minFlightTime;
		yield return new WaitForSeconds(seconds);
		_spawnExplosionAfterWarning = Object.Instantiate(_spawnExplosionAfterWarningPrefab);
		_spawnExplosionAfterWarning.transform.position = _targetPosition;
		_spawnExplosionAfterWarning.SetWarningDelay(_minFlightTime);
		_spawnExplosionAfterWarning.SetHellfireMultiplier(_hellfireDamageMultiplier);
		_spawnExplosionAfterWarning.Activate();
	}

	private IEnumerator ProjectileFlight()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(_spikeStart, 1f);
		float num = _targetPosition.x - _sourcePosition.x;
		float xThird = num / 3f;
		float yTarget = _outsideRangeYDistance + _sourcePosition.y;
		Vector2 vector = new Vector2(_sourcePosition.x + xThird, yTarget);
		float thirdOfFlightTime = _flightTime / 3f;
		RotateProjectileTowardTarget(vector);
		LeanTween.move(base.gameObject, vector, thirdOfFlightTime);
		yield return new WaitForSeconds(thirdOfFlightTime);
		LeanTween.move(to: new Vector2(_sourcePosition.x + xThird + xThird, yTarget), gameObject: base.gameObject, time: thirdOfFlightTime);
		yield return new WaitForSeconds(thirdOfFlightTime);
		Vector2 t3 = _targetPosition;
		RotateProjectileTowardTarget(t3);
		LeanTween.move(base.gameObject, t3, thirdOfFlightTime);
		SingletonController<AudioController>.Instance.PlaySFXClip(_spikeFalling, 1f);
		yield return new WaitForSeconds(thirdOfFlightTime);
		SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(_spikeLand, 1f, t3);
		yield return new WaitForSeconds(0.1f);
		Object.Destroy(base.gameObject);
	}

	private void RotateProjectileTowardTarget(Vector2 targetPosition)
	{
		Vector3 vector = base.transform.InverseTransformPoint(targetPosition);
		float z = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		base.transform.eulerAngles = new Vector3(0f, 0f, z);
	}

	internal void SetHellfireMultiplier(float hellfireDamageMultiplier)
	{
		_hellfireDamageMultiplier = hellfireDamageMultiplier;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		LeanTween.cancel(base.gameObject);
		if (_spawnExplosionAfterWarning != null)
		{
			Object.Destroy(_spawnExplosionAfterWarning.gameObject);
		}
	}
}
