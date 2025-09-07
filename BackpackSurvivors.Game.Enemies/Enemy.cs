using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Combat.EnemyAttacks;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Enemies.ExternalStats;
using BackpackSurvivors.Game.Enemies.Movement;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Health;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Pickups;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.DamageNumbers;
using UnityEngine;

namespace BackpackSurvivors.Game.Enemies;

public class Enemy : Character
{
	internal delegate void EnemyDamagedHandler(object sender, EnemyDamagedEventArgs e);

	internal delegate void EnemyDebuffedHandler(object sender, EnemyDebuffedEventArgs e);

	[SerializeField]
	private HitVFX[] _hitVfx;

	[SerializeField]
	private EnemySO _enemySO;

	[SerializeField]
	private HealthBarUI _healthBarUI;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private bool _rotateTowardsPlayer;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private KnockbackFeedback knockbackFeedback;

	[SerializeField]
	private LootBag _lootBag;

	[SerializeField]
	private bool _canTeleport = true;

	[SerializeField]
	private bool _spriteIsLookingToRight;

	[SerializeField]
	private EnemyWeaponInitializer[] _enemyWeaponInitializers;

	[Header("Elite")]
	[SerializeField]
	private GameObject _healthbarEliteBackdrop;

	[SerializeField]
	private Material _eliteMaterial;

	[SerializeField]
	private float _eliteHealthModifier = 2f;

	[SerializeField]
	private float _eliteDamageModifier = 1.5f;

	[SerializeField]
	private float _eliteScaleModifier = 1.5f;

	[SerializeField]
	private float _eliteLootModifier = 3f;

	internal int SpatialGroupID;

	internal int BatchID;

	private EnemyMovement _enemyMovement;

	private Stack<Enums.EnemyMovementType> _previousEnemyMovements = new Stack<Enums.EnemyMovementType>();

	private DamagePlayerOnTouch _damagePlayerOnTouch;

	private float _lootScaleFactor;

	private BackpackSurvivors.Game.Player.Player _playerToTarget;

	private bool _canTakeSpikeDamage = true;

	private Vector2 _originalScale;

	private bool _characterDiedExecuted;

	private TargetArrow _targetArrow;

	public int ExperienceValue;

	public float StopMovementTime;

	public bool IsCurrentlyAttacking;

	private Coroutine stopMovementRoutine;

	private DamageVisualizer _damageVisualizer => DamageVisualizer;

	public bool IsAlive { get; private set; }

	public int BaseId => _enemySO.Id;

	public Enums.Debuff.DebuffType[] DebuffImmunities => _enemySO.DebuffImmunities;

	public Enums.Enemies.EnemyType EnemyType => _enemySO.EnemyType;

	public Enums.Enemies.EnemyOnHitType EnemyOnHitType { get; private set; }

	public bool CanDropLoot { get; internal set; }

	public EnemySO BaseEnemy => _enemySO;

	public bool KillOnLevelCompleted => _enemySO.KillOnLevelCompleted;

	public EnemyMovement EnemyMovement => _enemyMovement;

	internal bool CanTakeSpikeDamage => _canTakeSpikeDamage;

	internal EnemyWeaponInitializer[] EnemyWeaponInitializers => _enemyWeaponInitializers;

	internal float Radius { get; private set; }

	internal Animator Animator => _animator;

	internal event EnemyDamagedHandler OnEnemyDamaged;

	internal event EnemyDebuffedHandler OnEnemyDebuffed;

	internal override Enums.Enemies.EnemyType GetCharacterType()
	{
		return EnemyType;
	}

	private void Awake()
	{
		Initialize();
	}

	private void Start()
	{
		_playerToTarget = SingletonController<GameController>.Instance.Player;
		InitBuffedStats();
		float difficultyModifier = 1f;
		if (SingletonController<DifficultyController>.Instance != null && SingletonController<AdventureController>.Instance != null && SingletonController<AdventureController>.Instance.ActiveAdventure != null)
		{
			difficultyModifier = SingletonController<DifficultyController>.Instance.GetExperienceMultiplierFromHellfire(SingletonController<AdventureController>.Instance.ActiveAdventure);
		}
		TimeBasedLevelController timeBasedLevelController = SingletonController<EnemyController>.Instance.TimeBasedLevelController;
		if (timeBasedLevelController != null)
		{
			InitExperienceValue(timeBasedLevelController.LevelExperienceModifier, difficultyModifier);
		}
		_originalScale = _spriteRenderer.transform.localScale;
		UpdateRadius();
	}

	internal void RunLogic()
	{
		RotateTowardsPlayer();
		if (!TeleportCloserToPlayer())
		{
			Move();
		}
	}

	private void Move()
	{
		if (_enemyMovement != null)
		{
			_enemyMovement.Move();
		}
	}

	private void UpdateRadius()
	{
		Radius = GetComponent<CircleCollider2D>().radius * base.transform.localScale.x;
	}

	public void UpgradeToElite()
	{
		UpgradeHealthToElite();
		UpgradeDamageToElite();
		UpgradeLootToElite();
		_spriteRenderer.material = _eliteMaterial;
		_spriteRenderer.transform.localScale = new Vector3(_spriteRenderer.transform.localScale.x * _eliteScaleModifier, _spriteRenderer.transform.localScale.y * _eliteScaleModifier, _spriteRenderer.transform.localScale.z);
		UpdateRadius();
	}

	public void UpgradeHealthToElite()
	{
		_healthbarEliteBackdrop.SetActive(value: true);
		base.HealthSystem.SetHealthMax(base.HealthSystem.GetHealthMax() * _eliteHealthModifier, setHealthToMax: true);
		_healthBarUI.transform.localScale = new Vector3(_healthBarUI.transform.localScale.x * _eliteScaleModifier, _healthBarUI.transform.localScale.y * _eliteScaleModifier, _healthBarUI.transform.localScale.z);
	}

	public void UpgradeDamageToElite()
	{
		_damagePlayerOnTouch.ScaleDamage(_eliteDamageModifier);
	}

	public void UpgradeLootToElite()
	{
		ScaleLoot(_eliteLootModifier);
	}

	internal override SpriteRenderer GetSpriteRenderer()
	{
		return _spriteRenderer;
	}

	internal void SetArrow(TargetArrow targetArrow)
	{
		_targetArrow = targetArrow;
	}

	internal override void AnimateSuicide()
	{
		base.AnimateSuicide();
		SetHealthBarVisibility(show: false);
		_damageVisualizer.enabled = false;
		if (!(_animator == null))
		{
			if (_animator.HasParameter("Suicide"))
			{
				_animator.SetTrigger("Suicide");
			}
			_animator.SetBool("IsDead", value: true);
		}
	}

	public void ShowSpawnAnimation()
	{
		if (!(_animator == null))
		{
			_animator.SetTrigger("BossSpawnAnimation");
		}
	}

	public void SpawnAudio()
	{
		SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(_enemySO.SpawnAudioClip, 1f, base.transform.position);
	}

	public void SetHealthBarVisibility(bool show)
	{
		_healthBarUI.gameObject.SetActive(show);
	}

	public override void SetCanAct(bool canAct)
	{
		base.SetCanAct(canAct);
		if (!(_animator == null) && _animator.ParameterExists("CanAct"))
		{
			_animator.SetBool("CanAct", canAct);
		}
	}

	public virtual void ResetToDefaultVisualState()
	{
	}

	public void StopMovementForAmountOfTime(float timeToStopMoving, bool fromAttack)
	{
		if (StopMovementTime < timeToStopMoving)
		{
			StopMovementTime = timeToStopMoving;
		}
		if (stopMovementRoutine != null)
		{
			StopCoroutine(stopMovementRoutine);
		}
		stopMovementRoutine = StartCoroutine(StopMovementUntillTime(fromAttack));
	}

	private IEnumerator StopMovementUntillTime(bool fromAttack)
	{
		float originalSpeed = base.CalculatedStats.GetValueOrDefault(Enums.ItemStatType.SpeedPercentage);
		EnemyMovement.SetCanMove(canMove: false);
		EnemyMovement.ChangeMovementSpeed(0f);
		EnemyMovement.StopMovement();
		EnemyMovement.PreventMovement();
		while (StopMovementTime > 0f)
		{
			yield return new WaitForSeconds(0.1f);
			StopMovementTime -= 0.1f;
		}
		if (fromAttack)
		{
			IsCurrentlyAttacking = false;
		}
		EnemyMovement.SetCanMove(canMove: true);
		EnemyMovement.AllowMovement();
		EnemyMovement.ChangeMovementSpeed(originalSpeed);
	}

	private bool TeleportCloserToPlayer()
	{
		if (!_canTeleport)
		{
			return false;
		}
		float distance = Vector3.Distance(base.transform.position, _playerToTarget.transform.position);
		if (SingletonController<EnemyController>.Instance.ShouldTeleportToPlayer(distance))
		{
			Vector2 teleportLocation = SingletonController<EnemyController>.Instance.GetTeleportLocation(base.transform.position, SingletonController<GameController>.Instance.PlayerPosition);
			_enemyMovement.Teleport(teleportLocation);
			return true;
		}
		return false;
	}

	public void ReplaceEnemyMovement(Enums.EnemyMovementType enemyMovementType, bool storeCurrentMovement = true, string waveChunkName = "")
	{
		if (storeCurrentMovement)
		{
			_previousEnemyMovements.Push(_enemyMovement.EnemyMovementType);
		}
		Rigidbody2D rigidBody = _enemyMovement.RigidBody;
		Object.DestroyImmediate(_enemyMovement);
		_enemyMovement = AddMovementComponent(enemyMovementType);
		_enemyMovement.SetRigidBody(rigidBody);
		_enemyMovement.WaveChunkName = waveChunkName;
		InitMovement();
	}

	public void RevertToPreviousEnemyMovement()
	{
		if (_previousEnemyMovements.Count == 0)
		{
			Debug.LogWarning("Trying to revert to a previous enemy movement without any previous movements stored");
			return;
		}
		Enums.EnemyMovementType enemyMovementType = _previousEnemyMovements.Pop();
		ReplaceEnemyMovement(enemyMovementType, storeCurrentMovement: false);
	}

	private EnemyMovement AddMovementComponent(Enums.EnemyMovementType enemyMovementType)
	{
		switch (enemyMovementType)
		{
		case Enums.EnemyMovementType.Burrowing:
			return base.gameObject.AddComponent<BurrowingMovement>();
		case Enums.EnemyMovementType.Charging:
			return base.gameObject.AddComponent<Charging>();
		case Enums.EnemyMovementType.FollowPlayer:
			return base.gameObject.AddComponent<FollowPlayerMovement>();
		case Enums.EnemyMovementType.FollowPlayerAndPause:
			return base.gameObject.AddComponent<FollowPlayerAndPause>();
		case Enums.EnemyMovementType.FollowPlayerUntilRange:
			return base.gameObject.AddComponent<FollowPlayerUntilRange>();
		case Enums.EnemyMovementType.FollowPlayerUntilRangeAndRunAway:
			return base.gameObject.AddComponent<FollowPlayerUntilRangeAndRunAway>();
		case Enums.EnemyMovementType.MoveInFixedDirection:
			return base.gameObject.AddComponent<LineSpawnMovement>();
		case Enums.EnemyMovementType.PassThroughPlayerLocation:
			return base.gameObject.AddComponent<PassThroughPlayerLocationMovement>();
		case Enums.EnemyMovementType.RunAwayFromPlayerUntilRange:
			return base.gameObject.AddComponent<RunAwayFromPlayerUntillRange>();
		case Enums.EnemyMovementType.RunAwayInRandomDirectionWhenPlayerInRange:
			return base.gameObject.AddComponent<RunAwayInRandomDirectionWhenPlayerInRange>();
		case Enums.EnemyMovementType.Stationary:
			return base.gameObject.AddComponent<Stationary>();
		case Enums.EnemyMovementType.MoveToFixedPoint:
			return base.gameObject.AddComponent<CircleSpawnMovement>();
		default:
			Debug.LogWarning(string.Format("EnemyMovementType {0} is not handled in {1}.{2}", enemyMovementType, "Enemy", "AddMovementComponent"));
			return null;
		}
	}

	public void SetEnemySortOrder(int sortOrder)
	{
		_spriteRenderer.sortingOrder = sortOrder;
	}

	public void SetCanTeleport(bool canTeleport)
	{
		_canTeleport = canTeleport;
	}

	private void RotateTowardsPlayer()
	{
		if (_rotateTowardsPlayer && IsAlive)
		{
			bool flag = SingletonController<GameController>.Instance.PlayerPosition.x > _spriteRenderer.transform.position.x;
			bool flipX = (_spriteIsLookingToRight ? (!flag) : flag);
			_spriteRenderer.flipX = flipX;
		}
	}

	internal void SetFixedSpriteFacingDirection(bool shouldFaceRight)
	{
		_rotateTowardsPlayer = false;
		bool flipX = (_spriteIsLookingToRight ? (!shouldFaceRight) : shouldFaceRight);
		_spriteRenderer.flipX = flipX;
	}

	internal void Initialize()
	{
		InitGuid();
		InitializeMovingAnimation();
		InitStats();
		InitMovement();
		InitializeHealthSystem();
		InitDamagePlayerOnTouch();
		InitLootBag();
		SetCanAct(canAct: true);
		SingletonController<EventController>.Instance.RegisterEnemy(this);
		IsAlive = true;
	}

	public void InitExperienceValue(float levelExperienceModifier, float difficultyModifier)
	{
		ExperienceValue = (int)((float)BaseEnemy.BaseExperience * levelExperienceModifier * difficultyModifier);
	}

	private void InitLootBag()
	{
		_lootBag = GetComponent<LootBag>();
		if (!(_lootBag == null))
		{
			_lootBag.Init(_enemySO.LootBagSO);
		}
	}

	private void InitDamagePlayerOnTouch()
	{
		_damagePlayerOnTouch = GetComponentInChildren<DamagePlayerOnTouch>();
		if (!(_damagePlayerOnTouch == null))
		{
			Dictionary<Enums.WeaponStatType, float> critInfo = new Dictionary<Enums.WeaponStatType, float>();
			DamageSO damageSO = GetDamageSO();
			_damagePlayerOnTouch.Init(base.CalculatedStats, damageSO, critInfo);
		}
	}

	private DamageSO GetDamageSO()
	{
		if (ExternalEnemyStats.ExternalEnemyStatsAvailable())
		{
			return ExternalEnemyStats.GetEnemyDamageSO(_enemySO.Id, _enemySO.DamageSO);
		}
		return _enemySO.DamageSO;
	}

	private void InitMovement()
	{
		_enemyMovement = GetComponent<EnemyMovement>();
		_enemyMovement.Init(GetCalculatedStat(Enums.ItemStatType.SpeedPercentage));
		_enemyMovement.SetCanMove(canMove: true);
	}

	private void InitStats()
	{
		base.CalculatedStatsWithSource = StatCalculator.CreateComplexItemStatDictionary();
		base.CalculatedDamageTypeValues = StatCalculator.CreateDamageTypeDictionary();
		InitEnemyStats();
		InitEnemyDamageTypes();
	}

	private void InitEnemyDamageTypes()
	{
		foreach (KeyValuePair<Enums.DamageType, float> damageTypeValue in _enemySO.DamageTypeValues)
		{
			base.CalculatedDamageTypeValues[damageTypeValue.Key] = damageTypeValue.Value;
		}
	}

	private void InitEnemyStats()
	{
		Dictionary<Enums.ItemStatType, float> enemyStats = GetEnemyStats();
		InitCalculatedStats(enemyStats);
	}

	private Dictionary<Enums.ItemStatType, float> GetEnemyStats()
	{
		if (ExternalEnemyStats.ExternalEnemyStatsAvailable())
		{
			return ExternalEnemyStats.GetExternalEnemyStats(_enemySO.Id, _enemySO.Stats.ToDictionary());
		}
		return _enemySO.Stats.ToDictionary();
	}

	public void InitializeHealthSystem()
	{
		base.HealthSystem = new HealthSystem(GetCalculatedStat(Enums.ItemStatType.Health));
		InitHealthEvents();
		if (_healthBarUI != null)
		{
			_healthBarUI.SetHealthSystem(base.HealthSystem, EnemyType);
		}
		if (GetCalculatedStat(Enums.ItemStatType.HealthRegeneration) > 0f)
		{
			UpdateHealthRegen();
		}
	}

	public void UpdateHealthRegen()
	{
		StopCoroutine(HealthRegenAsync());
		StartCoroutine(HealthRegenAsync());
	}

	private IEnumerator HealthRegenAsync()
	{
		while (!base.IsDead)
		{
			yield return new WaitForSeconds(5f);
			float calculatedStat = GetCalculatedStat(Enums.ItemStatType.HealthRegeneration);
			base.HealthSystem.Heal(calculatedStat);
		}
	}

	private void InitializeMovingAnimation()
	{
		if (!(_animator == null))
		{
			_animator.SetBool("IsMoving", value: true);
		}
	}

	internal override bool Damage(float damage, bool wasCrit, GameObject sourceForKnockback, float knockbackPower, WeaponSO weaponSource, Enums.DamageType damageType, Character damageSource = null)
	{
		if (!CanTakeDamage())
		{
			return false;
		}
		if (base.Damage(damage, wasCrit, sourceForKnockback, knockbackPower, weaponSource, damageType, damageSource))
		{
			SingletonController<WeaponDamageAndDPSController>.Instance.AddDamage(weaponSource, damage);
		}
		if (sourceForKnockback != null)
		{
			Knockback(sourceForKnockback.transform, knockbackPower);
		}
		DamageVisualizer.ShowDamagePopup(damage, wasCrit, Enums.CharacterType.Enemy, damageType);
		if (_hitVfx.Any() && sourceForKnockback != null)
		{
			int num = Random.Range(0, _hitVfx.Count());
			_hitVfx[num].ShowHit();
		}
		if (_enemySO.EnemyType == Enums.Enemies.EnemyType.Monster || _enemySO.EnemyType == Enums.Enemies.EnemyType.Minion)
		{
			LeanTween.cancel(_spriteRenderer.gameObject);
			float num2 = 1.5f;
			if (_enemySO.EnemyType == Enums.Enemies.EnemyType.Elite)
			{
				num2 = 1.2f;
			}
			_spriteRenderer.transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _spriteRenderer.transform.localScale.z);
			LeanTween.scale(_spriteRenderer.gameObject, new Vector3(_spriteRenderer.transform.localScale.x * num2, _spriteRenderer.transform.localScale.y * num2, _spriteRenderer.transform.localScale.z), 0.2f).setLoopPingPong(1);
		}
		this.OnEnemyDamaged?.Invoke(this, new EnemyDamagedEventArgs(this, wasCrit, weaponSource, damageType, damageSource));
		return true;
	}

	internal void Knockback(Transform sourceForKnockback, float knockbackPower)
	{
		if (_enemySO.CanBeKnockedBack)
		{
			float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.Knockback);
			knockbackFeedback.PlayFeedback(sourceForKnockback, knockbackPower * (calculatedStat + 1f), _enemySO.KnockbackDefense);
		}
	}

	public override void CharacterDamaged(float damageTaken, bool shouldTriggerAudioOnDamage)
	{
		if (damageTaken > float.Epsilon)
		{
			if (shouldTriggerAudioOnDamage)
			{
				SingletonController<AudioController>.Instance.PlayOnHitAudioOnPosition(EnemyOnHitType, base.transform.position);
			}
			if (!(_animator == null))
			{
				_animator.SetTrigger("OnHurt");
			}
		}
	}

	internal override void AddDebuff(DebuffHandler debuffHandler, Character sourceCharacter)
	{
		base.AddDebuff(debuffHandler, sourceCharacter);
	}

	internal override void TriggerDebuffsAddedEvent(List<Enums.Debuff.DebuffType> debuffsCaused)
	{
		base.TriggerDebuffsAddedEvent(debuffsCaused);
		this.OnEnemyDebuffed?.Invoke(this, new EnemyDebuffedEventArgs(this, debuffsCaused));
	}

	public override void CharacterDied()
	{
		if (!_enemySO.IsDummyTarget && !_characterDiedExecuted)
		{
			_healthBarUI.gameObject.SetActive(value: false);
			_characterDiedExecuted = true;
			if (_targetArrow != null)
			{
				SingletonCacheController.Instance.GetControllerByType<TargetArrowController>().RemoveArrow(_targetArrow);
			}
			StartCoroutine(CharacterDiedAsync());
			StartCoroutine(DestroyEnemyAfterAnimation());
			RaiseCharacterKilledEvent();
		}
	}

	private IEnumerator CharacterDiedAsync()
	{
		yield return null;
		DeactivateEnemy();
		if (_enemySO.EnemyOnDeathAudio != null && (_enemySO.EnemyType == Enums.Enemies.EnemyType.Boss || _enemySO.EnemyType == Enums.Enemies.EnemyType.Miniboss || RandomHelper.GetRollSuccess(0.2f)))
		{
			SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(_enemySO.EnemyOnDeathAudio, 0.8f, base.transform.position);
		}
		if (_animator != null)
		{
			_animator.SetBool("IsDead", value: true);
			_animator.SetTrigger("OnDeath");
		}
		yield return new WaitForEndOfFrame();
		TryDrops();
	}

	internal override Enums.Debuff.DebuffType[] GetDebuffImmunities()
	{
		return _enemySO.DebuffImmunities;
	}

	internal void DestroyWithoutKilling()
	{
		SingletonController<EnemyController>.Instance.RemoveEnemy(this);
		Object.Destroy(base.gameObject);
	}

	internal void TryDrops()
	{
		if (!(_lootBag == null) && CanDropLoot)
		{
			_lootBag.TryDrop(base.transform, _lootScaleFactor);
		}
	}

	private void DeactivateEnemy()
	{
		IsAlive = false;
		if (!_enemySO.IsDummyTarget)
		{
			_healthBarUI.enabled = false;
			_damageVisualizer.enabled = false;
			_enemyMovement.StopMovement();
			_enemyMovement.SetCanMove(canMove: false);
			MoveToPhasingLayer();
			EnemyWeaponInitializer[] enemyWeaponInitializers = _enemyWeaponInitializers;
			for (int i = 0; i < enemyWeaponInitializers.Length; i++)
			{
				enemyWeaponInitializers[i].enabled = false;
			}
			if (_damagePlayerOnTouch != null)
			{
				_damagePlayerOnTouch.SetCanAct(canAct: false);
			}
			RemoveHealthEvents();
		}
	}

	internal void MoveToPhasingLayer()
	{
		base.gameObject.layer = LayerMask.NameToLayer("PHASING_ENEMIES");
	}

	internal void MoveToSpecialMovementLayer()
	{
		base.gameObject.layer = LayerMask.NameToLayer("SPECIALMOVEMENT_ENEMIES");
	}

	private IEnumerator DestroyEnemyAfterAnimation()
	{
		yield return new WaitForSeconds(_enemySO.DeathAnimationDuration);
		Object.Destroy(base.gameObject);
	}

	internal void ScaleHealth(float scaleFactor, float hellfireScaleFactor, float levelDurationScaleFactor)
	{
		float num = _enemySO.Stats[Enums.ItemStatType.Health];
		if (_enemySO.EnemyType == Enums.Enemies.EnemyType.Miniboss || _enemySO.EnemyType == Enums.Enemies.EnemyType.Boss)
		{
			base.HealthSystem.SetHealthMax(num * hellfireScaleFactor, setHealthToMax: true);
		}
		else
		{
			base.HealthSystem.SetHealthMax(num * scaleFactor * hellfireScaleFactor * levelDurationScaleFactor, setHealthToMax: true);
		}
		if (EnemyType == Enums.Enemies.EnemyType.Elite)
		{
			UpgradeHealthToElite();
		}
	}

	internal void ScaleDamage(float damageScaleFactor)
	{
		if (_damagePlayerOnTouch != null)
		{
			_damagePlayerOnTouch.ScaleDamage(damageScaleFactor);
		}
		if (EnemyType == Enums.Enemies.EnemyType.Elite)
		{
			UpgradeDamageToElite();
		}
	}

	internal void ScaleMovementSpeed(float movementSpeedScaleFactor)
	{
		_enemyMovement.ScaleMovementSpeed(movementSpeedScaleFactor);
	}

	internal void ScaleLoot(float lootScaleFactor)
	{
		_lootScaleFactor = lootScaleFactor;
	}

	internal void SetColliderEnabled(bool enabled)
	{
		GetComponent<Collider2D>().enabled = enabled;
	}

	internal void SetDamageOnTouchCanAct(bool canAct)
	{
		_damagePlayerOnTouch.SetCanAct(canAct);
	}

	internal void SetCanTakeSpikeDamage(bool canTakeSpikeDamage)
	{
		_canTakeSpikeDamage = canTakeSpikeDamage;
	}

	private void OnDestroy()
	{
		RemoveHealthEvents();
	}

	internal void Reset()
	{
		RemoveHealthEvents();
		ResetDebuffs();
		Initialize();
	}

	internal virtual void AfterSpawning()
	{
	}

	internal virtual void BeforeSpawning()
	{
	}
}
