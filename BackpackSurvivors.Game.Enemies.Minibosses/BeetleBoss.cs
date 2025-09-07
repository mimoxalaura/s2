using System;
using System.Collections;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyMovement))]
internal class BeetleBoss : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private GameObject _rollingParticles;

	[SerializeField]
	private GameObject _spikeParticles;

	[SerializeField]
	private SpriteRenderer _enemySpriteRenderer;

	[SerializeField]
	private AudioClip _rollingStart;

	[SerializeField]
	private BeetleBossSpikeProjectile _beetleBossSpikeProjectilePrefab;

	[SerializeField]
	private int _projectileCount;

	[SerializeField]
	private Transform[] _spikeSpawnPoints;

	[SerializeField]
	private float _minSpikeDistance;

	[SerializeField]
	private float _maxSpikeDistance;

	[SerializeField]
	private KnockbackOnTouchEnter _knockbackOnTouch;

	private Enemy _enemy;

	private EnemyMovement _enemyMovement;

	private bool _isRolling;

	private bool _canAttack = true;

	private void Start()
	{
		_enemy = GetComponent<Enemy>();
		_enemyMovement = GetComponent<EnemyMovement>();
		_enemy.HealthSystem.OnDead += HealthSystem_OnDead;
		_enemy.OnEnemyDamaged += _enemy_OnEnemyDamaged;
		SingletonController<GameController>.Instance.Player.OnKilled += Player_OnKilled;
		SingletonController<GameController>.Instance.Player.OnCharacterRevived += Player_OnCharacterRevived;
		_animator.SetBool("IsMoving", value: true);
		StartCoroutine(RunBossAttacks());
	}

	private void _enemy_OnEnemyDamaged(object sender, EnemyDamagedEventArgs e)
	{
		_animator.SetTrigger("OnHurt");
	}

	private void Player_OnCharacterRevived(object sender, EventArgs e)
	{
		StartCoroutine(RunBossAttacks());
		_canAttack = false;
	}

	private void Player_OnKilled(object sender, KilledEventArgs e)
	{
		StopAllCoroutines();
		_canAttack = false;
	}

	private void HealthSystem_OnDead(object sender, EventArgs e)
	{
		_enemy.ResetDebuffs();
		_canAttack = false;
		_rollingParticles.SetActive(value: false);
		_animator.SetBool("Spiking", value: false);
		_animator.SetBool("Rolling", value: false);
		BeetleBossSpikeProjectile[] array = UnityEngine.Object.FindObjectsByType<BeetleBossSpikeProjectile>(FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
		StopAllCoroutines();
	}

	private IEnumerator RunBossAttacks()
	{
		yield return new WaitForSeconds(0.3f);
		_animator.SetBool("IsMoving", value: true);
		while (_enemy.CanAct && _canAttack)
		{
			yield return new WaitForSeconds(10f);
			AttackWithRoll();
			yield return new WaitForSeconds(10f);
			AttackWithSpikes();
		}
	}

	private void Update()
	{
		if (_isRolling)
		{
			float x = ((!_enemySpriteRenderer.flipX) ? 1 : (-1));
			_rollingParticles.transform.localScale = new Vector3(x, _rollingParticles.transform.localScale.y, _rollingParticles.transform.localScale.z);
		}
	}

	public void AttackWithSpikes()
	{
		_enemyMovement.SetCanMove(canMove: false);
		_enemyMovement.ToggleLockPosition(locked: true);
		_animator.SetBool("Spiking", value: true);
		_animator.SetTrigger("StartSpiking");
		StartCoroutine(AttackWithSpikesAsync());
	}

	private IEnumerator AttackWithSpikesAsync()
	{
		yield return new WaitForSeconds(1.4f);
		float hellfireDamageMultiplier = SingletonController<DifficultyController>.Instance.GetCurrentLevelEnemyDamageMultiplierFromHellfire();
		_spikeParticles.SetActive(value: true);
		float delayPerSpike = 5f / (float)_projectileCount;
		for (int i = 0; i < _projectileCount; i++)
		{
			if (_enemy.CanAct && _canAttack)
			{
				int num = UnityEngine.Random.Range(0, _spikeSpawnPoints.Length);
				Vector2 vector = _spikeSpawnPoints[num].position;
				UnityEngine.Random.Range(_minSpikeDistance, _maxSpikeDistance);
				Vector2 playerPosition = SingletonController<GameController>.Instance.PlayerPosition;
				BeetleBossSpikeProjectile beetleBossSpikeProjectile = UnityEngine.Object.Instantiate(_beetleBossSpikeProjectilePrefab, vector, Quaternion.identity);
				beetleBossSpikeProjectile.SetHellfireMultiplier(hellfireDamageMultiplier);
				beetleBossSpikeProjectile.StartFlying(playerPosition, vector, randomFlyTime: false);
			}
			yield return new WaitForSeconds(delayPerSpike);
		}
		yield return new WaitForSeconds(2.5f);
		_spikeParticles.SetActive(value: false);
		_animator.SetBool("Spiking", value: false);
		_enemy.SetCanAct(canAct: true);
		_enemyMovement.SetCanMove(canMove: true);
		_enemyMovement.ToggleLockPosition(locked: false);
	}

	public void AttackWithRoll()
	{
		_isRolling = true;
		base.gameObject.layer = LayerMask.NameToLayer("PHASING_ENEMIES");
		_animator.SetBool("IsMoving", value: false);
		_animator.SetBool("Rolling", value: true);
		_animator.SetTrigger("StartRolling");
		_enemyMovement.SetCanMove(canMove: false);
		_enemyMovement.ToggleLockPosition(locked: true);
		StartCoroutine(AttackWithRollAsync());
	}

	private IEnumerator AttackWithRollAsync()
	{
		yield return new WaitForSeconds(1.2f);
		_enemyMovement.SetCanMove(canMove: true);
		_enemyMovement.ToggleLockPosition(locked: false);
		_knockbackOnTouch.EnableKnockback = true;
		_enemyMovement.ChangeMovementSpeed(3f);
		_rollingParticles.SetActive(value: true);
		SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(_rollingStart, 1f, base.transform.position);
		yield return new WaitForSeconds(4.8f);
		_animator.SetTrigger("FinishRolling");
		_enemyMovement.SetCanMove(canMove: false);
		_enemyMovement.ToggleLockPosition(locked: true);
		yield return new WaitForSeconds(0.4f);
		_enemyMovement.ChangeMovementSpeed(1f);
		_enemyMovement.SetCanMove(canMove: true);
		_enemyMovement.ToggleLockPosition(locked: false);
		_rollingParticles.SetActive(value: false);
		_knockbackOnTouch.EnableKnockback = false;
		_animator.SetBool("Rolling", value: false);
		_animator.SetBool("IsMoving", value: true);
		base.gameObject.layer = LayerMask.NameToLayer("ENEMIES");
		_isRolling = false;
	}
}
