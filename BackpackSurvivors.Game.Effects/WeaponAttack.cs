using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class WeaponAttack : MonoBehaviour
{
	[SerializeField]
	public Projectile ProjectilePrefab;

	public bool DestroyDummyTargetOnDestroy;

	public bool DestroyDummySourceOnDestroy;

	private List<WeaponAttackEffect> _weaponAttackEffects;

	private List<DebuffHandler> _weaponAttackDebuffHandlers;

	private Vector2 _targetPosition;

	private bool _beingDestroyed;

	private float _projectileSpeed;

	[SerializeField]
	private float _pierceAmount;

	[SerializeField]
	private float _projectileDuration;

	private bool _canAct;

	private bool _canTriggerEffects;

	private bool _canTriggerDebuffs;

	private List<Character> _targetsHit = new List<Character>();

	private List<Character> _charactersToIgnoreOnHit = new List<Character>();

	private Character _source;

	private Character _target;

	private CombatWeapon _combatWeapon;

	public DamageInstance _damageInstance;

	private WeaponSO _weaponSO;

	private Projectile _projectile;

	private ProjectileVisualization _projectileVisualization;

	private ProjectileVisualizationOnPlayer _projectileVisualizationOnPlayer;

	private GameObject _emptyParentGameObject;

	private GameObject _emptyPlayerVisualizationParentGameObject;

	private AudioSource _flyingAudioSource;

	public DamageInstance DamageInstance => _damageInstance;

	public float MaxRange { get; private set; }

	public Vector2 TargetPosition => _targetPosition;

	public ProjectileVisualization ProjectileVisualization => _projectileVisualization;

	public Projectile Projectile => _projectile;

	public float FlightAffectTickTime => _weaponSO.FlightAffectTickTime;

	public Enums.CharacterType CharacterTypeForTriggerOnTouch => _weaponSO.CharacterTypeForTriggerOnTouch;

	public bool AllowMultipleHitsOnSameCharacter => _weaponSO.AllowMultipleHitsOnSameCharacter;

	public float KnockbackPower => _weaponSO.KnockbackPower;

	public event Action<List<Enemy>> OnEnemyHit;

	public event Action<CombatWeapon, Enemy> OnEnemyKilled;

	private void Awake()
	{
		_projectile = UnityEngine.Object.Instantiate(ProjectilePrefab, base.transform);
	}

	private void Update()
	{
		if (_canAct && !SingletonController<GameController>.Instance.GamePaused)
		{
			UpdatePosition();
			DestroyIfOutOfRange();
		}
	}

	public void Init(Character source, Character target, CombatWeapon combatWeapon, DamageInstance damageInstance)
	{
		_weaponSO = combatWeapon.WeaponInstance.BaseWeaponSO;
		_combatWeapon = combatWeapon;
		_source = source;
		_target = target;
		_damageInstance = damageInstance;
		_targetPosition = target.transform.position;
		SetProperties();
		StartDestroyAfterLifetimeExpired();
		SingletonController<EventController>.Instance.RegisterWeaponAttackEvents(this);
	}

	private void StartDestroyAfterLifetimeExpired()
	{
		if (_projectileDuration != 0f)
		{
			StartCoroutine(DestroyAfterLifetimeExpired());
		}
	}

	private void SetProperties()
	{
		Dictionary<Enums.WeaponStatType, float> weaponStats = GetWeaponStats();
		_projectileSpeed = weaponStats.GetValueOrDefault(Enums.WeaponStatType.ProjectileSpeed, 0f);
		_pierceAmount = ((_pierceAmount == 0f) ? weaponStats.GetValueOrDefault(Enums.WeaponStatType.Penetrating, 0f) : _pierceAmount);
		_projectileDuration = ((_projectileDuration == 0f) ? weaponStats.GetValueOrDefault(Enums.WeaponStatType.ProjectileDuration, 0f) : _projectileDuration);
		MaxRange = weaponStats.GetValueOrDefault(Enums.WeaponStatType.WeaponRange, 1f);
		if (!(_combatWeapon != null))
		{
			return;
		}
		_weaponAttackDebuffHandlers = _combatWeapon.WeaponAttackDebuffHandlers;
		_weaponAttackEffects = _combatWeapon.WeaponAttackEffects;
		foreach (WeaponAttackEffect weaponAttackEffect in _weaponAttackEffects)
		{
			weaponAttackEffect.HandlerOnTrigger.Init(this, _weaponAttackDebuffHandlers, _combatWeapon.WeaponInstance);
		}
	}

	public void Activate(Vector2 targetPosition, bool canTriggerEffects, bool canTriggerDebuffs, Character source, Character target)
	{
		StartCoroutine(ActivateAsync(targetPosition, canTriggerEffects, canTriggerDebuffs, source, target));
		StartCoroutine(ActivatePlayerVisualizationAsync(source, target));
		StartCoroutine(ActivateProjectileAudioAsync());
		StartCoroutine(ActivatePlayerVisualizationAudioAsync());
	}

	private IEnumerator ActivateAsync(Vector2 targetPosition, bool canTriggerEffects, bool canTriggerDebuffs, Character source, Character target)
	{
		if (Projectile == null)
		{
			Debug.LogWarning("Weapon attack has no visual prefab attached!");
			yield return null;
		}
		if (Projectile.ProjectileVisualization == null)
		{
			Debug.LogWarning("Weapon attack has no visual prefab attached!");
			yield return null;
		}
		Projectile.SetStats(GetWeaponStats(), _weaponSO);
		_emptyParentGameObject = new GameObject();
		if (Projectile.ProjectileMovementFreedom == Enums.ProjectileMovementFreedom.InWorld)
		{
			if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y))
			{
				_emptyParentGameObject.transform.position = Vector3.zero;
			}
			else
			{
				_emptyParentGameObject.transform.position = targetPosition;
			}
		}
		yield return new WaitForSeconds(Projectile.ProjectileSpawnDelay);
		_projectileVisualization = UnityEngine.Object.Instantiate(Projectile.ProjectileVisualization, _emptyParentGameObject.transform);
		Projectile.SetProjectileMovementComponent(_projectileVisualization, source, target);
		_projectileVisualization.Scale(Projectile.CalculateScaleModifier());
		Vector2 rotatedOffset = Vector2.zero;
		if (source != null && source.isActiveAndEnabled)
		{
			RotateProjectile(targetPosition, _projectileVisualization.transform, source.transform.position, out rotatedOffset);
			SetContinuousRotation(_projectileVisualization, Projectile.SelfRotationsPerSecond);
			rotatedOffset = FlipOffset(rotatedOffset, source);
			Transform parent = SetProjectileParent(source, target, rotatedOffset);
			FlipProjectileVisualization(parent);
			_canTriggerDebuffs = canTriggerDebuffs;
			_canTriggerEffects = canTriggerEffects;
			HandleEffectsOnTrigger();
			if (_canTriggerEffects)
			{
				StartCoroutine(StartHandleEffectsOnFlight());
			}
			_projectileVisualization.InitTriggers(CharacterTypeForTriggerOnTouch, source, targetPosition);
			_projectileVisualization.OnTriggerOnTouch += ProjectileVisualization_OnTriggerOnTouch;
			if (DamageInstance.DamageType != DamageInstance.CalculatedDamageType)
			{
				_projectileVisualization.SetMaterial(DamageInstance.CalculatedDamageType);
			}
			_projectileVisualization.SpawnAnimation();
			yield return null;
			_projectileVisualization.StartAttackLogic();
			_canAct = true;
		}
		yield return null;
	}

	private Vector2 FlipOffset(Vector2 rotatedOffset, Character source)
	{
		if (Projectile.ProjectileStartPointFlip == Enums.ProjectileStartPointFlip.NoNotFlip)
		{
			return rotatedOffset;
		}
		int num = (source.IsRotatedToRight() ? 1 : (-1));
		return new Vector2(rotatedOffset.x * (float)num, rotatedOffset.y);
	}

	private IEnumerator ActivatePlayerVisualizationAsync(Character source, Character target)
	{
		if (Projectile != null && Projectile.ProjectileVisualizationOnPlayer != null)
		{
			_emptyPlayerVisualizationParentGameObject = new GameObject();
			yield return new WaitForSeconds(Projectile.PlayerVisualizationSpawnDelay);
			SetPlayerVisualizationParent(source, target);
			_projectileVisualizationOnPlayer = UnityEngine.Object.Instantiate(Projectile.ProjectileVisualizationOnPlayer, _emptyPlayerVisualizationParentGameObject.transform);
			_projectileVisualizationOnPlayer.Init(source, target, Projectile);
			Vector2 rotatedOffset = Vector2.zero;
			RotatePlayerVisualization(target.transform.position, _emptyPlayerVisualizationParentGameObject.transform, source.transform.position, out rotatedOffset);
			yield return new WaitForSeconds(Projectile.PlayerVisualizationLifetime);
			_projectileVisualizationOnPlayer.DestroyAnimation();
			yield return new WaitForSeconds(Projectile.PlayerVisualizationDestructionDelay);
			UnityEngine.Object.Destroy(_emptyPlayerVisualizationParentGameObject);
		}
		yield return null;
	}

	private IEnumerator ActivateProjectileAudioAsync()
	{
		yield return new WaitForSeconds(Projectile.ProjectileTriggerAudioDelay);
		PlayProjectileAudio();
	}

	private void PlayProjectileAudio()
	{
		StartCoroutine(PlayProjectileAudioAsync());
		_flyingAudioSource = SingletonController<AudioController>.Instance.PlaySFXClip(Projectile.ProjectileFlyingAudio, 1f, 0f, AudioController.GetPitchVariation());
	}

	private IEnumerator PlayProjectileAudioAsync()
	{
		for (int i = 0; i < Projectile.ProjectileTriggerAudioCount; i++)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(Projectile.ProjectileTriggerAudio, Projectile.ProjectileTriggerAudioVolume, Projectile.ProjectileTriggerAudioOffset, AudioController.GetPitchVariation());
			yield return new WaitForSeconds(Projectile.ProjectileTriggerAudioDelayBetweenCount);
		}
	}

	private IEnumerator ActivatePlayerVisualizationAudioAsync()
	{
		yield return new WaitForSeconds(Projectile.PlayerVisualizationTriggerAudioOffset);
		PlayPlayerVisualizationAudio();
	}

	private void PlayPlayerVisualizationAudio()
	{
		SingletonController<AudioController>.Instance.PlaySFXClip(Projectile.PlayerVisualizationTriggerAudio, 1f, 0f, AudioController.GetPitchVariation());
	}

	private void SetPlayerVisualizationParent(Character source, Character target)
	{
		_ = Projectile.ProjectileStartPointFlip;
		switch (Projectile.PlayerVisualizationFreedom)
		{
		case Enums.ProjectileMovementFreedom.AtPlayer:
			switch (Projectile.PlayerVisualizationFlip)
			{
			case Enums.ProjectileFlip.None:
				_emptyPlayerVisualizationParentGameObject.transform.SetParent(source.transform);
				_emptyPlayerVisualizationParentGameObject.transform.position = source.transform.position;
				break;
			case Enums.ProjectileFlip.FlipXBasedOnTargetPosition:
				_emptyPlayerVisualizationParentGameObject.transform.SetParent(source.transform);
				_emptyPlayerVisualizationParentGameObject.transform.position = source.transform.position;
				break;
			case Enums.ProjectileFlip.FlipXBasedOnPlayerDirection:
				_emptyPlayerVisualizationParentGameObject.transform.SetParent(source.GetSpriteRenderer().transform);
				_emptyPlayerVisualizationParentGameObject.transform.position = source.GetSpriteRenderer().transform.position;
				break;
			}
			break;
		case Enums.ProjectileMovementFreedom.AtEnemy:
			Debug.LogWarning("Player Visualization has Freedom of AtEnemy - unhandled!");
			break;
		case Enums.ProjectileMovementFreedom.InWorld:
			_emptyPlayerVisualizationParentGameObject.transform.position = source.transform.position;
			break;
		}
	}

	private Transform SetProjectileParent(Character source, Character target, Vector2 rotatedOffset)
	{
		Transform result = null;
		bool flag = Projectile.ProjectileStartPointFlip == Enums.ProjectileStartPointFlip.FlipWithPlayer;
		switch (Projectile.ProjectileMovementFreedom)
		{
		case Enums.ProjectileMovementFreedom.AtPlayer:
		{
			Transform parent = (flag ? source.AttackPoint : source.NonFlippingAttackPoint);
			_emptyParentGameObject.transform.SetParent(parent, worldPositionStays: false);
			_emptyParentGameObject.transform.localPosition = AddProjectileOffset(_emptyParentGameObject.transform.localPosition);
			result = _emptyParentGameObject.transform;
			break;
		}
		case Enums.ProjectileMovementFreedom.AtEnemy:
			if (target == null)
			{
				return null;
			}
			_emptyParentGameObject.transform.SetParent(target.transform);
			_emptyParentGameObject.transform.localPosition = Vector2.zero;
			result = _emptyParentGameObject.transform;
			break;
		case Enums.ProjectileMovementFreedom.InWorld:
			switch (Projectile.ProjectileStartPosition)
			{
			case Enums.ProjectileStartPosition.OnPlayer:
			{
				Vector3 position = (flag ? source.AttackPoint.position : source.NonFlippingAttackPoint.position);
				_emptyParentGameObject.transform.position = position;
				_emptyParentGameObject.transform.localPosition = AddProjectileOffset(_emptyParentGameObject.transform.localPosition, rotatedOffset);
				result = _emptyParentGameObject.transform;
				break;
			}
			case Enums.ProjectileStartPosition.OnTarget:
				if (target == null)
				{
					return null;
				}
				_emptyParentGameObject.transform.position = AddProjectileOffset(target.transform.position);
				result = _emptyParentGameObject.transform;
				break;
			case Enums.ProjectileStartPosition.OnPosition:
			{
				Transform transform = (flag ? source.AttackPoint : source.NonFlippingAttackPoint);
				_emptyParentGameObject.transform.position = transform.transform.position;
				_emptyParentGameObject.transform.localPosition = AddProjectileOffset(_emptyParentGameObject.transform.localPosition);
				result = _emptyParentGameObject.transform;
				break;
			}
			case Enums.ProjectileStartPosition.OnCustomPosition:
				_emptyParentGameObject.transform.position = Projectile.StartPositionLocation;
				_emptyParentGameObject.transform.localPosition = AddProjectileOffset(_emptyParentGameObject.transform.localPosition);
				result = _emptyParentGameObject.transform;
				break;
			}
			break;
		}
		return result;
	}

	private Vector3 AddProjectileOffset(Vector3 position)
	{
		Vector2 vector = position;
		float x = Projectile.StartPositionOffset.x;
		float y = Projectile.StartPositionOffset.y;
		vector.x += x;
		vector.y += y;
		return vector;
	}

	private Vector3 AddProjectileOffset(Vector3 position, Vector2 rotatedOffset)
	{
		Vector2 vector = position;
		float x = rotatedOffset.x;
		float y = rotatedOffset.y;
		vector.x += x;
		vector.y += y;
		return vector;
	}

	private Vector3 AddPlayerVisualizationOffset(Vector3 position)
	{
		Vector2 vector = position;
		float x = Projectile.PlayerVisualizationStartPositionOffset.x;
		float y = Projectile.PlayerVisualizationStartPositionOffset.y;
		vector.x += x;
		vector.y += y;
		return vector;
	}

	internal void SetTargetPosition(Vector2 sourcePosition, Vector2 targetPosition, float range, float spreadOffsetDegrees)
	{
		_targetPosition = Projectile.GetTargetPosition(sourcePosition, targetPosition, range, spreadOffsetDegrees);
	}

	public void ChangeMaterial(Material elementMaterial)
	{
		GetComponent<SpriteRenderer>().material = elementMaterial;
	}

	internal void SetCustomStartPosition(Vector2 position)
	{
		Projectile.SetCustomStartPosition(position);
	}

	internal void RotateProjectile(Vector2 targetPosition, Transform transformToRotate, Vector2 sourcePosition, out Vector2 rotatedOffset)
	{
		if (Projectile.ProjectileRotation == Enums.ProjectileRotation.RotatingConstantly || Projectile.ProjectileRotation == Enums.ProjectileRotation.None)
		{
			rotatedOffset = Projectile.StartPositionOffset;
			return;
		}
		float angle = 0f;
		transformToRotate.eulerAngles = Projectile.GetRotationAngle(sourcePosition, targetPosition, out angle);
		rotatedOffset = Projectile.GetRotatedOffset(angle, Projectile.ProjectileStartPointFlip);
	}

	internal void RotatePlayerVisualization(Vector2 targetPosition, Transform transformToRotate, Vector2 sourcePosition, out Vector2 rotatedOffset)
	{
		if (Projectile.ProjectileRotation == Enums.ProjectileRotation.RotatingConstantly || Projectile.ProjectileRotation == Enums.ProjectileRotation.None)
		{
			rotatedOffset = Projectile.StartPositionOffset;
			return;
		}
		float angle = 0f;
		if (Projectile.PlayerVisualizationRotation != Enums.ProjectileRotation.None)
		{
			transformToRotate.eulerAngles = Projectile.GetRotationAngle(sourcePosition, targetPosition, out angle);
		}
		rotatedOffset = Projectile.GetRotatedOffset(angle, Projectile.ProjectileStartPointFlip);
	}

	internal void FlipProjectileVisualization(Transform parent)
	{
		if (!(parent == null))
		{
			bool flag = false;
			switch (Projectile.ProjectileFlip)
			{
			case Enums.ProjectileFlip.None:
				return;
			case Enums.ProjectileFlip.FlipXBasedOnPlayerDirection:
				flag = ((_source.GetCharacterType() != Enums.Enemies.EnemyType.Player) ? _source.GetSpriteRenderer().flipX : (!SingletonController<GameController>.Instance.Player.IsRotatedToRight()));
				break;
			case Enums.ProjectileFlip.FlipXBasedOnTargetPosition:
				flag = _targetPosition.x < _projectile.GetStartPosition(_targetPosition).x;
				break;
			}
			int num = ((!flag) ? 1 : (-1));
			Vector3 localScale = parent.localScale;
			LeanTweenExt.LeanScale(to: new Vector3(localScale.x * (float)num, localScale.y, localScale.z), transform: parent.transform, time: 0f);
		}
	}

	internal void SetContinuousRotation(ProjectileVisualization _projectileVisualization, float selfRotationsPerSecond)
	{
		if (Projectile.ProjectileRotation == Enums.ProjectileRotation.RotatingConstantly)
		{
			_projectileVisualization.SetContinuousRotation(selfRotationsPerSecond);
		}
	}

	private void ProjectileVisualization_OnTriggerOnTouch(object sender, TriggerOnTouchEventArgs e)
	{
		HandleOnHit(e.TriggeredOn);
	}

	private void ProjectileVisualization_OnTriggerOnStay(object sender, TriggerOnStayEventArgs e)
	{
		HandleOnHit(e.TriggeredOn);
	}

	private void UpdatePosition()
	{
		if (_projectileVisualization != null && _projectileVisualization.isActiveAndEnabled)
		{
			_projectileVisualization.transform.position = Projectile.GetNewPosition(_projectileVisualization.transform.position, _targetPosition);
		}
	}

	public void HandleEffectsOnFlight()
	{
		if (!_canTriggerEffects)
		{
			return;
		}
		foreach (WeaponAttackEffect item in _weaponAttackEffects.Where((WeaponAttackEffect x) => x.WeaponAttackEffectTrigger == Enums.WeaponAttackEffectTrigger.OnFlight))
		{
			item.HandlerOnTrigger.Activate(_projectileVisualization.transform, _target, _combatWeapon, _source);
		}
	}

	public IEnumerator StartHandleEffectsOnFlight()
	{
		while (!_beingDestroyed)
		{
			yield return new WaitForSeconds(FlightAffectTickTime);
			HandleEffectsOnFlight();
		}
	}

	public void HandleEffectsOnTrigger()
	{
		if (!_canTriggerEffects)
		{
			return;
		}
		foreach (WeaponAttackEffect item in _weaponAttackEffects.Where((WeaponAttackEffect x) => x.WeaponAttackEffectTrigger == Enums.WeaponAttackEffectTrigger.OnStart))
		{
			item.HandlerOnTrigger.Activate(_projectileVisualization.transform, _target, _combatWeapon, _source);
		}
	}

	private void HandleEffectsOnDestroy()
	{
		if (!_canTriggerEffects || _combatWeapon == null)
		{
			return;
		}
		foreach (WeaponAttackEffect item in _weaponAttackEffects.Where((WeaponAttackEffect x) => x.WeaponAttackEffectTrigger == Enums.WeaponAttackEffectTrigger.OnDestroy))
		{
			item.HandlerOnTrigger.Activate(_projectileVisualization.transform, _target, _combatWeapon, _source);
		}
	}

	internal void AddEnemiesToIgnoreOnHit(IEnumerable<Character> characters)
	{
		_charactersToIgnoreOnHit.AddRange(characters);
	}

	public void HandleEffectsOnHit(Character enemyHit)
	{
		if (!_canTriggerEffects)
		{
			return;
		}
		foreach (WeaponAttackEffect item in _weaponAttackEffects.Where((WeaponAttackEffect x) => x.WeaponAttackEffectTrigger == Enums.WeaponAttackEffectTrigger.OnHit))
		{
			item.HandlerOnTrigger.Activate(_projectileVisualization.transform, enemyHit, _combatWeapon, _source);
		}
	}

	public bool HandleDebuffsOnHit(Character enemyHit)
	{
		if (!_canTriggerDebuffs)
		{
			return false;
		}
		if (_weaponAttackDebuffHandlers == null)
		{
			return false;
		}
		if (!_weaponAttackDebuffHandlers.Any())
		{
			return false;
		}
		List<Enums.Debuff.DebuffType> list = new List<Enums.Debuff.DebuffType>();
		foreach (DebuffHandler weaponAttackDebuffHandler in _weaponAttackDebuffHandlers)
		{
			DebuffHandler debuffHandler = EffectsHelper.CopyDebuffHandler(weaponAttackDebuffHandler, _combatWeapon, enemyHit);
			enemyHit.AddDebuff(debuffHandler, _source);
			list.Add(debuffHandler.DebuffType);
		}
		enemyHit.TriggerDebuffsAddedEvent(list);
		return true;
	}

	public void HandleDamageOnHit(Character characterHit)
	{
		if (base.isActiveAndEnabled)
		{
			bool wasCrit;
			float damage = DamageEngine.CalculateDamage(GetWeaponStats(), GetSourceStats(), characterHit.CalculatedStats, DamageInstance, characterHit.GetCharacterType(), out wasCrit);
			bool damageWasDone = characterHit.Damage(damage, wasCrit, base.gameObject, KnockbackPower, _weaponSO, DamageInstance.CalculatedDamageType, _source);
			if (characterHit.IsDead && characterHit is Enemy)
			{
				this.OnEnemyKilled?.Invoke(_combatWeapon, characterHit as Enemy);
			}
			if (ShouldHandleLifedrain(damageWasDone))
			{
				HandleLifeDrain(damage);
			}
			if (ShouldHandleStun(GetSourceStats(), characterHit))
			{
				HandleStun(characterHit);
			}
		}
	}

	public void HandleOnHit(Character characterHit)
	{
		if (characterHit == null || !characterHit.isActiveAndEnabled || characterHit.IsDead || _charactersToIgnoreOnHit.Contains(characterHit) || (float)_targetsHit.Count > _pierceAmount)
		{
			return;
		}
		if (!characterHit.IsEnemy && !SingletonController<GameController>.Instance.Player.CanTakeDamage())
		{
			if ((float)(_targetsHit.Count + 1) > _pierceAmount)
			{
				ExecuteDestroy(destroyedByHit: false);
			}
		}
		else if (AllowMultipleHitsOnSameCharacter || !_targetsHit.Contains(characterHit))
		{
			_targetsHit.Add(characterHit);
			TriggerCharacterHit(characterHit);
			HandleDamageOnHit(characterHit);
			HandleDebuffsOnHit(characterHit);
			HandleEffectsOnHit(characterHit);
			HandleChaining();
			Vector3 position = characterHit.transform.position;
			Vector3 vector = SingletonController<GameController>.Instance.PlayerPosition;
			if ((position - vector).magnitude < 20f)
			{
				SingletonController<AudioController>.Instance.PlaySFXClipAtPosition(Projectile.ProjectileHitAudio, 1f, position);
			}
			SingletonController<AudioController>.Instance.StopSFX(_flyingAudioSource);
			if ((float)_targetsHit.Count > _pierceAmount)
			{
				ExecuteDestroy(destroyedByHit: true);
			}
		}
	}

	private void HandleChaining()
	{
		switch (_projectile.ProjectileChaining)
		{
		case Enums.ProjectileChaining.None:
			break;
		case Enums.ProjectileChaining.ChainToNearestEnemy:
			ChainToNearestEnemy();
			break;
		default:
			Debug.LogWarning(string.Format("ProjectileChaining enum {0} is not handled in {1}.{2}", _projectile.ProjectileChaining, "WeaponAttack", "HandleChaining"));
			break;
		}
	}

	private void ChainToNearestEnemy()
	{
		List<Enemy> enemiesToIgnore = (from t in _targetsHit
			where t is Enemy
			select t as Enemy).ToList();
		Enemy enemyWithinRange = SingletonCacheController.Instance.GetControllerByType<EnemyController>().GetEnemyWithinRange(_projectileVisualization.transform.position, MaxRange, enemiesToIgnore);
		if (enemyWithinRange != null)
		{
			_targetPosition = enemyWithinRange.transform.position;
			Vector2 rotatedOffset = Vector2.zero;
			RotateProjectile(_targetPosition, _projectileVisualization.transform, _projectileVisualization.transform.position, out rotatedOffset);
		}
	}

	private void TriggerCharacterHit(Character characterHit)
	{
		if (characterHit is Enemy)
		{
			TriggerEnemyHit();
		}
	}

	private void TriggerEnemyHit()
	{
		List<Enemy> list = new List<Enemy>();
		foreach (Character item in _targetsHit)
		{
			if (!(item as Enemy == null))
			{
				list.Add(item as Enemy);
			}
		}
		this.OnEnemyHit?.Invoke(list);
	}

	private bool ShouldHandleStun(Dictionary<Enums.ItemStatType, float> dictionary, Character characterHit)
	{
		if (characterHit.IsEnemy)
		{
			switch (((Enemy)characterHit).EnemyType)
			{
			case Enums.Enemies.EnemyType.Miniboss:
				return false;
			case Enums.Enemies.EnemyType.Boss:
				return false;
			}
		}
		float num = UnityEngine.Random.Range(0f, 1f);
		return dictionary.TryGet(Enums.ItemStatType.StunChancePercentage, 0f) > num;
	}

	private void HandleStun(Character characterHit)
	{
		DebuffHandler debuffHandler = new DebuffHandler();
		debuffHandler.Init(GameDatabaseHelper.GetDebuffSO(Enums.Debuff.DebuffType.Stun), _combatWeapon, characterHit);
		characterHit.AddDebuff(debuffHandler, _source);
	}

	private bool ShouldHandleLifedrain(bool damageWasDone)
	{
		if (!damageWasDone)
		{
			return false;
		}
		Dictionary<Enums.WeaponStatType, float> weaponStats = GetWeaponStats();
		if (!weaponStats.ContainsKey(Enums.WeaponStatType.LifeDrainPercentage))
		{
			return false;
		}
		return weaponStats[Enums.WeaponStatType.LifeDrainPercentage] != 0f;
	}

	private void HandleLifeDrain(float damage)
	{
		_source.HandleBrotatoLifeDrain(_combatWeapon.WeaponStats[Enums.WeaponStatType.LifeDrainPercentage]);
	}

	private IEnumerator DestroyAfterLifetimeExpired()
	{
		if (!_beingDestroyed)
		{
			yield return new WaitForSeconds(_projectileDuration);
			ExecuteDestroy(destroyedByHit: false);
			_beingDestroyed = true;
		}
	}

	internal void ExecuteDestroy(bool destroyedByHit)
	{
		if (!(base.gameObject == null) && base.gameObject.activeSelf && !_beingDestroyed)
		{
			StartCoroutine(DestroyAfterDelay(destroyedByHit));
		}
	}

	private IEnumerator DestroyAfterDelay(bool destroyedByHit)
	{
		if (base.isActiveAndEnabled)
		{
			if (_projectileVisualization != null && destroyedByHit)
			{
				_projectileVisualization.gameObject.GetComponent<IProjectileMovement>().ToggleMovement(allowMovement: false);
				_projectileVisualization.HitAnimation();
			}
			if (_projectileVisualization != null && !destroyedByHit)
			{
				_projectileVisualization.DestroyAnimation();
			}
			yield return new WaitForSeconds(Projectile.ProjectileVisualizationDestructionDelay);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void DestroyIfOutOfRange()
	{
		if (!(_projectileVisualization == null) && !_projectile.IgnoreRangeLimit)
		{
			bool num = _projectile.ProjectileMovementComponent.TargetPositionReached(_projectileVisualization.transform.position, _targetPosition);
			if (num && _projectile.StopRotatingOnMovementEnd)
			{
				_projectileVisualization.StopRotation();
			}
			if (num && _projectile.ProjectileMovementComponent.TargetPositionReached(_projectileVisualization.transform.position, _targetPosition))
			{
				ExecuteDestroy(destroyedByHit: false);
			}
		}
	}

	private Dictionary<Enums.WeaponStatType, float> GetWeaponStats()
	{
		if (_combatWeapon != null)
		{
			return _combatWeapon.WeaponStats;
		}
		return new Dictionary<Enums.WeaponStatType, float>();
	}

	private Dictionary<Enums.ItemStatType, float> GetSourceStats()
	{
		if (_source != null)
		{
			return _source.CalculatedStats;
		}
		return new Dictionary<Enums.ItemStatType, float>();
	}

	private void OnDestroy()
	{
		HandleEffectsOnDestroy();
		StopAllCoroutines();
		if (_projectileVisualization != null)
		{
			_projectileVisualization.OnTriggerOnTouch -= ProjectileVisualization_OnTriggerOnTouch;
			if (_projectileVisualization != null)
			{
				UnityEngine.Object.Destroy(_projectileVisualization.gameObject);
			}
			if (_projectileVisualizationOnPlayer != null)
			{
				_projectileVisualizationOnPlayer.InitiateDestruction(_emptyPlayerVisualizationParentGameObject);
			}
		}
		if (_emptyParentGameObject != null)
		{
			UnityEngine.Object.Destroy(_emptyParentGameObject);
		}
		if (_projectile != null)
		{
			UnityEngine.Object.Destroy(_projectile.gameObject);
		}
		if (DestroyDummyTargetOnDestroy && _target != null)
		{
			UnityEngine.Object.Destroy(_target.gameObject);
		}
		if (DestroyDummySourceOnDestroy && _source != null)
		{
			UnityEngine.Object.Destroy(_source.gameObject);
		}
	}
}
