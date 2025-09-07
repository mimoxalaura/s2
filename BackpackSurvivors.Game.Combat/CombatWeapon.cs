using System;
using System.Collections;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Combat.Events;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Settings;
using UnityEngine;

namespace BackpackSurvivors.Game.Combat;

public class CombatWeapon : MonoBehaviour
{
	public delegate void WeaponAttackedHandler(object sender, WeaponAttackEventArgs e);

	public Guid UniqueWeaponKey;

	private EnemyController _enemyController;

	private Character _sourceCharacter;

	private float _attackDelay;

	private bool _attemptingAttack;

	private bool _isPaused;

	private bool _isVisibleWeapon;

	private bool _weaponReadyNotified;

	public WeaponInstance WeaponInstance { get; private set; }

	public Dictionary<Enums.WeaponStatType, float> WeaponStats { get; private set; }

	public List<WeaponAttackEffect> WeaponAttackEffects { get; private set; }

	public List<DebuffHandler> WeaponAttackDebuffHandlers { get; private set; }

	public string Name { get; private set; }

	public Sprite Icon { get; private set; }

	public Enums.WeaponType WeaponType { get; private set; }

	public float CurrentCooldown { get; private set; }

	public float Cooldown { get; private set; }

	public int AttackCount { get; private set; }

	public event WeaponAttackedHandler OnWeaponAttack;

	public event WeaponAttackedHandler OnBeforeWeaponAttack;

	public event WeaponController.WeaponReadyHandler OnWeaponReady;

	private EnemyController GetEnemyController()
	{
		if (_enemyController == null)
		{
			_enemyController = SingletonCacheController.Instance.GetControllerByType<EnemyController>();
		}
		return _enemyController;
	}

	public void Init(WeaponInstance weaponInstance, float attackDelay, Character sourceCharacter)
	{
		WeaponInstance = weaponInstance;
		WeaponStats = WeaponInstance.CalculatedStats;
		Name = WeaponInstance.Name;
		Icon = WeaponInstance.Icon;
		WeaponType = WeaponInstance.BaseWeaponType;
		Cooldown = WeaponStats[Enums.WeaponStatType.CooldownTime];
		CurrentCooldown = Cooldown;
		WeaponAttackEffects = WeaponInstance.WeaponAttackEffects;
		WeaponAttackDebuffHandlers = WeaponInstance.WeaponAttackDebuffHandlers;
		_sourceCharacter = sourceCharacter;
		_attackDelay = attackDelay;
		UniqueWeaponKey = Guid.NewGuid();
		ResetAttackCount();
		SingletonController<EventController>.Instance.RegisterCombatWeaponEvents(this);
		_isVisibleWeapon = SingletonController<BackpackController>.Instance.GetRarestWeaponInBackpack() == WeaponInstance;
	}

	public IEnumerator Attack()
	{
		_attemptingAttack = true;
		bool flag = HasTargetWithinRange() && _sourceCharacter.CanAct;
		if (WeaponInstance.BaseWeaponSO.IsPermanentEffect)
		{
			flag = flag && AttackCount == 0;
		}
		while (!flag)
		{
			yield return new WaitForSeconds(0.1f);
			if (this == null)
			{
				_attemptingAttack = false;
				yield break;
			}
			flag = HasTargetWithinRange() && _sourceCharacter.CanAct;
			if (WeaponInstance.BaseWeaponSO.IsPermanentEffect)
			{
				flag = flag && AttackCount == 0;
			}
		}
		HandleBeforeAttack();
		Enums.ProjectileSpreading projectileSpreading = WeaponInstance.BaseWeaponSO.ProjectileSpreading;
		float delayBetweenProjectiles = ((projectileSpreading == Enums.ProjectileSpreading.Spread) ? 0f : WeaponInstance.WeaponAttackPrefab.ProjectilePrefab.DelayBetweenProjectiles);
		float offsetDegrees = ((projectileSpreading == Enums.ProjectileSpreading.Spread) ? WeaponInstance.BaseWeaponSO.SpreadOffsetDegrees : 0f);
		float projectileCount = ((WeaponInstance.BaseWeaponType == Enums.WeaponType.Melee) ? 1 : ((int)Mathf.Max(WeaponStats[Enums.WeaponStatType.ProjectileCount], 1f)));
		yield return new WaitForSeconds(_attackDelay);
		for (int i = 0; (float)i < projectileCount; i++)
		{
			if (_isPaused)
			{
				continue;
			}
			_attemptingAttack = true;
			Character[] targets = GetTargets();
			if (targets != null)
			{
				float totalOffsetDegrees = GetTotalOffsetDegrees(offsetDegrees, i);
				Character[] array = targets;
				foreach (Character character in array)
				{
					if (character != null)
					{
						Attack(character, totalOffsetDegrees);
					}
				}
			}
			yield return new WaitForSeconds(delayBetweenProjectiles);
		}
		_attemptingAttack = false;
		HandleAfterAttack();
	}

	public void ScaleWeaponDamage(float damageScale)
	{
		WeaponInstance.ScaleWeaponDamage(damageScale);
	}

	public void ResetAttackCount()
	{
		AttackCount = 0;
	}

	private float GetTotalOffsetDegrees(float offsetDegrees, int projectileNumber)
	{
		int num = ((projectileNumber % 2 == 0) ? 1 : (-1));
		return (float)((projectileNumber + 1) / 2) * offsetDegrees * (float)num;
	}

	private void Attack(Character target, float spreadOffsetDegrees)
	{
		_attemptingAttack = true;
		switch (WeaponInstance.BaseAttackType)
		{
		case Enums.AttackTargetingType.AttackClosestEnemy:
		case Enums.AttackTargetingType.AttackRandomEnemy:
		case Enums.AttackTargetingType.AttackPlayer:
			AttackCharacter(target, _sourceCharacter, spreadOffsetDegrees);
			break;
		case Enums.AttackTargetingType.AttackPlayerDirection:
		case Enums.AttackTargetingType.AttackPlayerAim:
		case Enums.AttackTargetingType.None:
			AttackDirection(_sourceCharacter, spreadOffsetDegrees, target);
			break;
		case Enums.AttackTargetingType.TargetCursorDONOTUSE:
		case Enums.AttackTargetingType.AttackDummy:
		case Enums.AttackTargetingType.AttackCardinalDirection:
			AttackDummy(target, _sourceCharacter, spreadOffsetDegrees);
			break;
		}
	}

	private void HandleBeforeAttack()
	{
		_sourceCharacter.IsAttacking = true;
		if (_isVisibleWeapon)
		{
			LeanTween.delayedCall(WeaponInstance.BaseWeaponSO.DelayBeforePlayerAttackAnimation, (Action)delegate
			{
				_sourceCharacter.IsAttackingWithVisualWeapon = true;
			});
		}
		this.OnBeforeWeaponAttack?.Invoke(this, new WeaponAttackEventArgs(this));
	}

	public void HandleAfterAttack()
	{
		this.OnWeaponAttack?.Invoke(this, new WeaponAttackEventArgs(this));
		CurrentCooldown = WeaponStats.TryGet(Enums.WeaponStatType.CooldownTime, 1f);
		if (_sourceCharacter.GetCharacterType() != Enums.Enemies.EnemyType.Player)
		{
			((Enemy)_sourceCharacter).IsCurrentlyAttacking = false;
		}
		_weaponReadyNotified = false;
		AttackCount++;
	}

	private void AttackCharacter(Character target, Character source, float spreadOffsetDegrees)
	{
		float range = WeaponStats.TryGet(Enums.WeaponStatType.WeaponRange, 1f);
		WeaponAttack weaponAttack = InitWeaponAttack(source, target);
		weaponAttack.SetTargetPosition(source.transform.position, target.transform.position, range, spreadOffsetDegrees);
		weaponAttack.Activate(weaponAttack.TargetPosition, canTriggerEffects: true, canTriggerDebuffs: true, source, target);
	}

	private void AttackDummy(Character dummyTarget, Character source, float spreadOffsetDegrees)
	{
		WeaponStats.TryGet(Enums.WeaponStatType.WeaponRange, 1f);
		WeaponAttack weaponAttack = InitWeaponAttack(source, dummyTarget);
		weaponAttack.Activate(weaponAttack.TargetPosition, canTriggerEffects: true, canTriggerDebuffs: true, source, dummyTarget);
		weaponAttack.DestroyDummyTargetOnDestroy = true;
	}

	private void AttackDirection(Character source, float spreadOffsetDegrees, Character dummyTarget)
	{
		float range = WeaponStats.TryGet(Enums.WeaponStatType.WeaponRange, 1f);
		WeaponAttack weaponAttack = InitWeaponAttack(source, dummyTarget);
		weaponAttack.DestroyDummyTargetOnDestroy = true;
		Vector2 targetPosition = default(Vector2);
		switch (WeaponInstance.BaseAttackType)
		{
		case Enums.AttackTargetingType.AttackPlayerDirection:
		{
			int num = (source.IsRotatedToRight() ? 1 : 0);
			targetPosition = new Vector2(num, 0f);
			break;
		}
		case Enums.AttackTargetingType.AttackPlayerAim:
			targetPosition = default(Vector2);
			break;
		}
		weaponAttack.SetTargetPosition(source.transform.position, targetPosition, range, spreadOffsetDegrees);
		weaponAttack.Activate(weaponAttack.TargetPosition, canTriggerEffects: true, canTriggerDebuffs: true, source, dummyTarget);
	}

	private WeaponAttack InitWeaponAttack(Character source, Character target)
	{
		WeaponAttack weaponAttack;
		if (WeaponInstance.BaseWeaponSO.WeaponType == Enums.WeaponType.Melee)
		{
			weaponAttack = UnityEngine.Object.Instantiate(WeaponInstance.WeaponAttackPrefab, source.transform);
			weaponAttack.transform.SetParent(null);
		}
		else
		{
			weaponAttack = UnityEngine.Object.Instantiate(WeaponInstance.WeaponAttackPrefab, base.transform.position, Quaternion.identity);
		}
		weaponAttack.Init(source, target, this, WeaponInstance.DamageInstance);
		return weaponAttack;
	}

	public void DecreaseCooldown(float time)
	{
		if (!_isPaused)
		{
			CurrentCooldown -= time;
			if (CurrentCooldown <= 0f && !_weaponReadyNotified)
			{
				_weaponReadyNotified = true;
				this.OnWeaponReady?.Invoke(this, new WeaponReadyEventArgs(this));
			}
		}
	}

	public bool CanAttack()
	{
		if (!_isPaused && CurrentCooldown <= 0f)
		{
			return !_attemptingAttack;
		}
		return false;
	}

	internal void SetIsPaused(bool isPaused)
	{
		_isPaused = isPaused;
	}

	public void Refresh(WeaponInstance weaponInstance, Character sourceCharacter)
	{
		WeaponInstance = weaponInstance;
		WeaponStats = WeaponInstance.CalculatedStats;
		Cooldown = WeaponStats[Enums.WeaponStatType.CooldownTime];
		WeaponAttackEffects = WeaponInstance.WeaponAttackEffects;
		WeaponAttackDebuffHandlers = WeaponInstance.WeaponAttackDebuffHandlers;
	}

	private bool HasTargetWithinRange()
	{
		float range = WeaponStats.TryGet(Enums.WeaponStatType.WeaponRange, 0f);
		switch (WeaponInstance.BaseAttackType)
		{
		case Enums.AttackTargetingType.AttackClosestEnemy:
			if (SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting == Enums.Targeting.Manual || (SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting == Enums.Targeting.AutomaticButManualOnHotkey && SingletonController<InputController>.Instance.ShiftIsDown))
			{
				return true;
			}
			return GetEnemyController().GetEnemyWithinRange(base.transform.position, range) != null;
		case Enums.AttackTargetingType.AttackRandomEnemy:
			return GetEnemyController().GetRandomEnemyWithinRange(base.transform.position, range) != null;
		case Enums.AttackTargetingType.AttackPlayer:
			return PlayerWithinRange(base.transform.position, range);
		case Enums.AttackTargetingType.TargetCursorDONOTUSE:
			return true;
		case Enums.AttackTargetingType.AttackPlayerAim:
			return true;
		case Enums.AttackTargetingType.AttackPlayerDirection:
			return true;
		case Enums.AttackTargetingType.AttackDummy:
			return true;
		case Enums.AttackTargetingType.AttackCardinalDirection:
			return true;
		case Enums.AttackTargetingType.None:
			return true;
		default:
			Debug.LogError(string.Format("Case {0} is not handled in {1}.{2}()", WeaponInstance.BaseAttackType, "CombatWeapon", "HasTargetWithinRange"));
			return false;
		}
	}

	private Character[] GetTargets()
	{
		Character[] result = null;
		switch (WeaponInstance.BaseAttackType)
		{
		case Enums.AttackTargetingType.AttackClosestEnemy:
			result = ((SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting != Enums.Targeting.Manual && (SingletonController<SettingsController>.Instance.GameplaySettingsController.Targeting != Enums.Targeting.AutomaticButManualOnHotkey || !SingletonController<InputController>.Instance.ShiftIsDown)) ? GetClosestEnemy(applyEnemyPriority: true) : CreateAndGetCursorDirection(_sourceCharacter));
			break;
		case Enums.AttackTargetingType.AttackRandomEnemy:
			result = GetRandomEnemy();
			break;
		case Enums.AttackTargetingType.AttackPlayer:
			result = GetPlayer();
			break;
		case Enums.AttackTargetingType.AttackPlayerDirection:
			result = CreateAndGetPlayerDirection(_sourceCharacter);
			break;
		case Enums.AttackTargetingType.AttackPlayerAim:
			result = CreateAndGetPlayerAim(_sourceCharacter);
			break;
		case Enums.AttackTargetingType.AttackDummy:
			result = new Character[1] { SingletonController<EnemyController>.Instance.CreateAndGetDummyEnemy() };
			break;
		case Enums.AttackTargetingType.AttackCardinalDirection:
			result = CreateAndGetCardinalDirection(_sourceCharacter);
			break;
		case Enums.AttackTargetingType.TargetCursorDONOTUSE:
			result = CreateAndGetCursorDirection(_sourceCharacter);
			break;
		default:
			Debug.LogError(string.Format("Case {0} is not handled in {1}.{2}()", WeaponInstance.BaseAttackType, "CombatWeapon", "Attack"));
			break;
		case Enums.AttackTargetingType.None:
			break;
		}
		return result;
	}

	private Character[] GetPlayer()
	{
		return new Character[1] { SingletonController<GameController>.Instance.Player };
	}

	private bool PlayerWithinRange(Vector2 origin, float range)
	{
		BackpackSurvivors.Game.Player.Player player = SingletonController<GameController>.Instance.Player;
		return Vector2.Distance(origin, player.transform.position) <= range;
	}

	private Character[] GetClosestEnemy(bool applyEnemyPriority)
	{
		if (!base.isActiveAndEnabled)
		{
			return new Character[0];
		}
		Enemy enemyWithinRange = GetEnemyController().GetEnemyWithinRange(base.transform.position, WeaponStats.TryGet(Enums.WeaponStatType.WeaponRange, 0f), applyEnemyPriority);
		return new Character[1] { enemyWithinRange };
	}

	private Character[] GetRandomEnemy()
	{
		Enemy randomEnemyWithinRange = GetEnemyController().GetRandomEnemyWithinRange(base.transform.position, WeaponStats.TryGet(Enums.WeaponStatType.WeaponRange, 0f));
		return new Character[1] { randomEnemyWithinRange };
	}

	private Character[] CreateAndGetPlayerAim(Character source)
	{
		return new Character[0];
	}

	private Character[] CreateAndGetPlayerDirection(Character source)
	{
		int num = (source.IsRotatedToRight() ? 1 : (-1));
		Enemy enemy = UnityEngine.Object.Instantiate(position: new Vector2(1000 * num, source.transform.position.y), original: SingletonController<GameDatabase>.Instance.GameDatabaseSO.DummyEnemy, rotation: Quaternion.identity);
		return new Character[1] { enemy };
	}

	private Character[] CreateAndGetCardinalDirection(Character source)
	{
		List<Character> list = new List<Character>();
		Enums.CardinalDirection[] cardinalDirections = WeaponInstance.BaseWeaponSO.CardinalDirections;
		foreach (Enums.CardinalDirection cardinalDirection in cardinalDirections)
		{
			Vector2 vector = default(Vector2);
			switch (cardinalDirection)
			{
			case Enums.CardinalDirection.MovingNorth:
				vector = new Vector2(source.transform.position.x, source.transform.position.y + 1000f);
				break;
			case Enums.CardinalDirection.MovingEast:
				vector = new Vector2(source.transform.position.x + 1000f, source.transform.position.y);
				break;
			case Enums.CardinalDirection.MovingSouth:
				vector = new Vector2(source.transform.position.x, source.transform.position.y - 1000f);
				break;
			case Enums.CardinalDirection.MovingWest:
				vector = new Vector2(source.transform.position.x - 1000f, source.transform.position.y);
				break;
			case Enums.CardinalDirection.MovingNorthEast:
				vector = new Vector2(source.transform.position.x + 1000f, source.transform.position.y + 1000f);
				break;
			case Enums.CardinalDirection.MovingSouthEast:
				vector = new Vector2(source.transform.position.x + 1000f, source.transform.position.y - 1000f);
				break;
			case Enums.CardinalDirection.MovingSouthWest:
				vector = new Vector2(source.transform.position.x - 1000f, source.transform.position.y - 1000f);
				break;
			case Enums.CardinalDirection.MovingNorthWest:
				vector = new Vector2(source.transform.position.x - 1000f, source.transform.position.y + 1000f);
				break;
			}
			Enemy item = UnityEngine.Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.DummyEnemy, vector, Quaternion.identity);
			list.Add(item);
		}
		return list.ToArray();
	}

	private Character[] CreateAndGetCursorDirection(Character source)
	{
		Camera component = GameObject.Find("Main Camera").GetComponent<Camera>();
		Vector2 zero = Vector2.zero;
		if (SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard)
		{
			Vector2 vector = source.transform.position - component.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
			vector.Normalize();
			zero = new Vector2(source.transform.position.x, source.transform.position.y) + vector * -50f;
		}
		else
		{
			Vector2 vector2 = SingletonController<InputController>.Instance.ControllerAimVector;
			if (vector2.magnitude == 0f)
			{
				vector2 = Vector2.right;
			}
			zero = new Vector2(source.transform.position.x, source.transform.position.y) - vector2 * -50f;
		}
		Enemy enemy = UnityEngine.Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.DummyEnemy, zero, Quaternion.identity);
		return new Character[1] { enemy };
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}
