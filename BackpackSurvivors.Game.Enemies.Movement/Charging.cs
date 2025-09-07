using System;
using System.Collections;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Movement;

internal class Charging : EnemyMovement
{
	[SerializeField]
	private float _chargeUpTime;

	[SerializeField]
	private float _chargeSpeedMultiplier;

	[SerializeField]
	private float _chargeSpeedMultiplierDuration;

	[SerializeField]
	private float _followPlayerDuration;

	[SerializeField]
	private Enemy _baseEnemy;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private string _defaultLayerName;

	[SerializeField]
	private string _phasingLayerName;

	[SerializeField]
	private GameObject _chargingTrail;

	private bool _isChargingAtPlayer;

	private void Awake()
	{
		EnemyMovementType = Enums.EnemyMovementType.Charging;
	}

	protected override void AfterStart()
	{
		_baseEnemy.OnKilled += Enemy_OnCharacterKilled;
		StartCoroutine(StartFollowPlayer());
	}

	private void Enemy_OnCharacterKilled(object sender, EventArgs e)
	{
		StopAllCoroutines();
		StopMovement();
		_isChargingAtPlayer = false;
		_executeMovement = true;
	}

	private IEnumerator StartFollowPlayer()
	{
		yield return new WaitForSeconds(_followPlayerDuration);
		StartCoroutine(StartChargingUp());
	}

	private IEnumerator StartChargingUp()
	{
		PreventMovement();
		LeanTween.color(_spriteRenderer.gameObject, Color.red, _chargeUpTime);
		yield return new WaitForSeconds(_chargeUpTime);
		_spriteRenderer.color = Color.white;
		_baseEnemy.SetCanTeleport(canTeleport: false);
		AllowMovement();
		StartCoroutine(StartChargingDuration());
	}

	private IEnumerator StartChargingDuration()
	{
		_isChargingAtPlayer = true;
		base.gameObject.layer = LayerMask.NameToLayer("PHASING_ENEMIES");
		_chargingTrail.SetActive(value: true);
		yield return new WaitForSeconds(_chargeSpeedMultiplierDuration);
		base.gameObject.layer = LayerMask.NameToLayer(_defaultLayerName);
		_chargingTrail.SetActive(value: false);
		_baseEnemy.SetCanTeleport(canTeleport: true);
		_isChargingAtPlayer = false;
		StartCoroutine(StartFollowPlayer());
	}

	internal override void CalculateNewPosition()
	{
		if (SingletonController<GameController>.Instance.Player.IsDead)
		{
			PreventMovement();
			return;
		}
		Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
		float num = (_isChargingAtPlayer ? _chargeSpeedMultiplier : 1f);
		Vector3 vector = Vector3.MoveTowards(base.transform.position, playerPosition, base.MoveSpeed * num * Time.fixedDeltaTime * _movementScale);
		SetNewPosition(vector);
	}

	internal override bool MovementShouldIgnoreCollisions()
	{
		return false;
	}
}
