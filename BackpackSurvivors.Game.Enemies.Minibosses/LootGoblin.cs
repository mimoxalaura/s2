using System;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Health.Events;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Loot;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies.Minibosses;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(EnemyMovement))]
internal class LootGoblin : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private SpriteRenderer _enemySpriteRenderer;

	[SerializeField]
	private LootBag _lootBagOnKilled;

	[Header("Drops")]
	[SerializeField]
	private float _dropChance;

	[SerializeField]
	private float _dropTickTime;

	[SerializeField]
	private LootBag _lootBagForRandomDropsWhileRunning;

	[SerializeField]
	private LootBagSO _lootBagSOForRandomDropsWhileRunning;

	[SerializeField]
	private AudioClip _lootDrop;

	[SerializeField]
	private AudioClip _lootExplosion;

	[SerializeField]
	private float _minimumDistanceToPlayToDropLoot;

	[SerializeField]
	private int _maxHealthLootModifier = 20;

	[SerializeField]
	private WeaponSO _weaponSOForSelfDamage;

	[SerializeField]
	private ParticleSystem _coinsDropParticles;

	[SerializeField]
	private ParticleSystem _groundDropParticles;

	[SerializeField]
	private AudioSource _continuousCoinDroppingAudioSource;

	[Header("Movement")]
	[SerializeField]
	private float _movementPauseChance;

	[SerializeField]
	private float _movementPauseTickTime;

	[Header("Death")]
	[SerializeField]
	private GameObject[] DeactiveOnDeath;

	private Enemy _enemy;

	private EnemyMovement _enemyMovement;

	private float _maxHealth;

	private int _lootAvailableModifier;

	private float _percentHealthLostPerDrop;

	private Vector2 _previousLocation = Vector2.zero;

	private AudioSource _coinDropAudioSource;

	internal Enemy Enemy => _enemy;

	private void Awake()
	{
		_enemy = GetComponent<Enemy>();
		SetContinuousCoinDropAudioSourceVolume();
	}

	private void SetContinuousCoinDropAudioSourceVolume()
	{
		float volume = SingletonController<AudioController>.Instance.GetVolume(Enums.AudioType.Master);
		float volume2 = SingletonController<AudioController>.Instance.GetVolume(Enums.AudioType.SFX);
		_continuousCoinDroppingAudioSource.volume *= volume * volume2;
	}

	private void Start()
	{
		_enemyMovement = GetComponent<EnemyMovement>();
		_enemy.HealthSystem.OnDead += HealthSystem_OnDead;
		_enemy.HealthSystem.OnDamaged += HealthSystem_OnDamaged;
		_maxHealth = _enemy.HealthSystem.GetHealthMax();
		_lootAvailableModifier = _maxHealthLootModifier;
		_percentHealthLostPerDrop = 1f / (float)_maxHealthLootModifier;
		_enemy.CanDropLoot = true;
		_enemy.SetCanTeleport(canTeleport: false);
		_lootBagForRandomDropsWhileRunning.Init(_lootBagSOForRandomDropsWhileRunning);
		_enemy.IsMoving = false;
		_enemyMovement.OnStartedRunning += EnemyMovement_OnStartedRunning;
		_enemyMovement.OnStoppedRunning += EnemyMovement_OnStoppedRunning;
		SingletonController<GameController>.Instance.OnPauseUpdated += GameController_OnPauseUpdated;
		_animator.SetBool("IsMoving", value: false);
		_enemy.ScaleLoot(_lootAvailableModifier * (SingletonController<DifficultyController>.Instance.ActiveDifficulty + 1));
		SetCoinEmissions(emit: false);
	}

	private void GameController_OnPauseUpdated(bool gamePaused)
	{
		if (gamePaused)
		{
			_continuousCoinDroppingAudioSource.Pause();
		}
		if (!gamePaused)
		{
			_continuousCoinDroppingAudioSource.UnPause();
		}
	}

	private void EnemyMovement_OnStoppedRunning(object sender, EventArgs e)
	{
		_animator.SetBool("IsMoving", value: false);
		SetCoinEmissions(emit: false);
	}

	private void EnemyMovement_OnStartedRunning(object sender, EventArgs e)
	{
		_animator.SetBool("IsMoving", value: true);
		SetCoinEmissions(emit: true);
	}

	private void HealthSystem_OnDamaged(object sender, DamageTakenEventArgs e)
	{
		_lootBagForRandomDropsWhileRunning.TryDrop(base.transform, 1f);
		_coinDropAudioSource = SingletonController<AudioController>.Instance.PlaySFXClip(_lootDrop, 1f, 1f, AudioController.GetPitchVariation());
		_lootAvailableModifier--;
		_enemy.ScaleLoot(_lootAvailableModifier * (SingletonController<DifficultyController>.Instance.ActiveDifficulty + 1));
	}

	private void HealthSystem_OnDead(object sender, EventArgs e)
	{
		_enemyMovement.SetCanMove(canMove: false);
		_enemyMovement.StopMovement();
		_enemyMovement.PreventMovement();
		_enemyMovement.ScaleMovementSpeed(0f);
		_enemyMovement.ToggleLockPosition(locked: true);
		SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(_lootExplosion, 1f, base.transform.position);
		SingletonController<AudioController>.Instance.StopSFX(_coinDropAudioSource);
		_continuousCoinDroppingAudioSource.Stop();
		SingletonController<GameController>.Instance.OnPauseUpdated -= GameController_OnPauseUpdated;
		SetCoinEmissions(emit: false);
		GameObject[] deactiveOnDeath = DeactiveOnDeath;
		for (int i = 0; i < deactiveOnDeath.Length; i++)
		{
			deactiveOnDeath[i].SetActive(value: false);
		}
		StopAllCoroutines();
	}

	private void SetCoinEmissions(bool emit)
	{
		_coinsDropParticles.enableEmission = emit;
		_groundDropParticles.enableEmission = emit;
	}

	private void OnDestroy()
	{
		_enemyMovement.OnStartedRunning -= EnemyMovement_OnStartedRunning;
		_enemyMovement.OnStoppedRunning -= EnemyMovement_OnStoppedRunning;
		_enemy.HealthSystem.OnDead -= HealthSystem_OnDead;
		_enemy.HealthSystem.OnDamaged -= HealthSystem_OnDamaged;
		SingletonController<GameController>.Instance.OnPauseUpdated -= GameController_OnPauseUpdated;
	}
}
