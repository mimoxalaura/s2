using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Combat.ProjectileMovements;
using BackpackSurvivors.Game.Combat.ProjectileMovements.Interfaces;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Shared.Extensions;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class Projectile : MonoBehaviour
{
	[Header("Base")]
	[SerializeField]
	public Enums.ProjectileMovementFreedom ProjectileMovementFreedom;

	[SerializeField]
	public Enums.ProjectileStartPosition ProjectileStartPosition;

	[SerializeField]
	public Enums.ProjectileStartPointFlip ProjectileStartPointFlip;

	[SerializeField]
	public Enums.ProjectileMovement ProjectileMovement;

	[SerializeField]
	public Enums.ProjectileRotation ProjectileRotation;

	[SerializeField]
	public Enums.ProjectileFlip ProjectileFlip;

	[SerializeField]
	public Enums.SizeScaleSource SizeScaleSource;

	[SerializeField]
	public float DelayBetweenProjectiles = 0.2f;

	[SerializeField]
	public bool IgnoreRangeLimit;

	[Header("Prefabs")]
	[SerializeField]
	public ProjectileVisualization ProjectileVisualization;

	[SerializeField]
	public ProjectileVisualizationOnPlayer ProjectileVisualizationOnPlayer;

	[Header("Waving Towards Target Movement")]
	[SerializeField]
	public float Magnitude;

	[SerializeField]
	public float Frequency;

	[Header("Circling Transform Movement")]
	[SerializeField]
	public float RotationsPerSecond;

	[SerializeField]
	public float DistanceFromTransform;

	[SerializeField]
	public float DistanceIncreasePerSecond;

	[Header("Boomerang Movement")]
	[SerializeField]
	public AnimationCurve BoomerangSpeedCurve;

	[Header("Thrown Movement")]
	[SerializeField]
	public float ArcHeight;

	[Header("Rotation")]
	[SerializeField]
	public float SelfRotationsPerSecond;

	[SerializeField]
	public float RotationFix;

	[SerializeField]
	public float Angle;

	[SerializeField]
	public bool StopRotatingOnMovementEnd;

	[Header("Start Position")]
	[SerializeField]
	[Tooltip("This offset determine where the projectile will spawn, relative to the StartPosition property")]
	public Vector2 StartPositionOffset;

	[SerializeField]
	[Tooltip("This offset determine where the projectile will spawn, only used when StartPosition = 'OnCustomPosition'")]
	public Vector2 StartPositionLocation;

	[Header("Delay")]
	[SerializeField]
	[Tooltip("This determines how long we should wait untill actually spawning the projectile")]
	public float ProjectileSpawnDelay;

	[SerializeField]
	[Tooltip("This determines how long we should wait untill actually destrying the projectile after destruction was triggered")]
	public float ProjectileVisualizationDestructionDelay;

	[Header("Visuals")]
	[SerializeField]
	public Enums.ProjectileMovementFreedom PlayerVisualizationFreedom;

	[SerializeField]
	public Enums.ProjectileFlip PlayerVisualizationFlip;

	[SerializeField]
	public Enums.ProjectileRotation PlayerVisualizationRotation;

	[SerializeField]
	[Tooltip("This determines how long we should wait untill actually spawning the projectile")]
	public float PlayerVisualizationSpawnDelay;

	[SerializeField]
	[Tooltip("This offset determine where the player visualization will spawn, relative to the source transform position property")]
	public Vector2 PlayerVisualizationStartPositionOffset;

	[SerializeField]
	[Tooltip("This determines how long we should wait untill actually destrying the projectile after destruction was triggered")]
	public float PlayerVisualizationDestructionDelay;

	[SerializeField]
	[Tooltip("This determines how long the visualization should be active")]
	public float PlayerVisualizationLifetime;

	[Header("Audio")]
	[SerializeField]
	public AudioClip ProjectileTriggerAudio;

	[SerializeField]
	public float ProjectileTriggerAudioVolume = 1f;

	[SerializeField]
	public int ProjectileTriggerAudioCount = 1;

	[SerializeField]
	public float ProjectileTriggerAudioDelayBetweenCount;

	[SerializeField]
	public float ProjectileTriggerAudioDelay;

	[SerializeField]
	public float ProjectileTriggerAudioOffset;

	[SerializeField]
	public float ProjectileTriggerAudioPitch = 1f;

	[SerializeField]
	public AudioClip PlayerVisualizationTriggerAudio;

	[SerializeField]
	public float PlayerVisualizationTriggerAudioOffset;

	public AudioClip ProjectileHitAudio;

	public AudioClip ProjectileFlyingAudio;

	private IProjectileMovement _projectileMovement;

	private Dictionary<Enums.WeaponStatType, float> _weaponStats;

	private WeaponSO _weaponSO;

	private float _scaleModifier;

	private float _projectileSpeed;

	public Enums.ProjectileChaining ProjectileChaining => _weaponSO.ProjectileChaining;

	public Enums.ProjectileSpreading ProjectileSpreading => _weaponSO.ProjectileSpreading;

	internal IProjectileMovement ProjectileMovementComponent => _projectileMovement;

	public void SetProjectileMovementComponent(ProjectileVisualization projectileVisualizationInstance, Character source, Character target)
	{
		AddProjectileMovementComponent(projectileVisualizationInstance, source, target);
	}

	public void SetStats(Dictionary<Enums.WeaponStatType, float> weaponStats, WeaponSO weaponSO)
	{
		_weaponStats = weaponStats;
		_weaponSO = weaponSO;
		_projectileSpeed = _weaponStats.TryGet(Enums.WeaponStatType.ProjectileSpeed, 1f);
	}

	private void AddProjectileMovementComponent(ProjectileVisualization projectileVisualizationInstance, Character source, Character target)
	{
		switch (ProjectileMovement)
		{
		case Enums.ProjectileMovement.None:
			projectileVisualizationInstance.gameObject.AddComponent<NoMovement>();
			break;
		case Enums.ProjectileMovement.HeadingTowardsTarget:
			projectileVisualizationInstance.gameObject.AddComponent<HeadingTowardsTargetMovement>();
			break;
		case Enums.ProjectileMovement.RotatingAroundStartPosition:
			AddCirclingTransformComponent(projectileVisualizationInstance, source, target);
			break;
		case Enums.ProjectileMovement.Waving:
			AddWavingTowardsTargetComponent(projectileVisualizationInstance);
			break;
		case Enums.ProjectileMovement.MovingIntoAngleDirection:
			AddHeadingInAngledDirectionMovement(projectileVisualizationInstance);
			break;
		case Enums.ProjectileMovement.MovingFacingPlayerAim:
			projectileVisualizationInstance.gameObject.AddComponent<HeadingTowardsPlayerAimMovement>();
			break;
		case Enums.ProjectileMovement.Boomerang:
			AddBoomerangMovement(projectileVisualizationInstance);
			break;
		case Enums.ProjectileMovement.Thrown:
			AddThrownMovement(projectileVisualizationInstance);
			break;
		default:
			Debug.LogWarning(string.Format("Enum value {0} is not handled in {1}.{2}", ProjectileMovement, "WeaponAttack", "AddProjectileMovementComponent"));
			break;
		case Enums.ProjectileMovement.MovingFacingPlayerDirection:
			break;
		}
		_projectileMovement = projectileVisualizationInstance.gameObject.GetComponent<IProjectileMovement>();
	}

	private void AddThrownMovement(ProjectileVisualization projectileVisualizationInstance)
	{
		projectileVisualizationInstance.gameObject.AddComponent<ThrownMovement>().Init(base.transform.position, ArcHeight);
	}

	private void AddBoomerangMovement(ProjectileVisualization projectileVisualizationInstance)
	{
		if (base.isActiveAndEnabled)
		{
			BoomerangMovement boomerangMovement = projectileVisualizationInstance.gameObject.AddComponent<BoomerangMovement>();
			_weaponStats.TryGetValue(Enums.WeaponStatType.WeaponRange, out var value);
			boomerangMovement.Init(base.transform.position, value, BoomerangSpeedCurve);
		}
	}

	private void AddWavingTowardsTargetComponent(ProjectileVisualization projectileVisualizationInstance)
	{
		if (base.isActiveAndEnabled)
		{
			projectileVisualizationInstance.gameObject.AddComponent<WavingTowardsTargetMovement>().Init(base.transform.position, Magnitude, Frequency);
		}
	}

	private void AddCirclingTransformComponent(ProjectileVisualization projectileVisualizationInstance, Character source, Character target)
	{
		if (!(source == null) && source.isActiveAndEnabled)
		{
			CirclingTransformMovement circlingTransformMovement = projectileVisualizationInstance.gameObject.AddComponent<CirclingTransformMovement>();
			int num = 1;
			_weaponStats.TryGetValue(Enums.WeaponStatType.WeaponRange, out var value);
			Transform circleCentre = null;
			switch (ProjectileMovementFreedom)
			{
			case Enums.ProjectileMovementFreedom.AtPlayer:
				circleCentre = source.transform;
				break;
			case Enums.ProjectileMovementFreedom.InWorld:
				circleCentre = UnityEngine.Object.Instantiate(SingletonController<GameDatabase>.Instance.GameDatabaseSO.DummyTransformPrefab, source.transform.position, Quaternion.identity).transform;
				break;
			case Enums.ProjectileMovementFreedom.AtEnemy:
				circleCentre = target.transform;
				break;
			}
			circlingTransformMovement.Init(ProjectileMovementFreedom, circleCentre, RotationsPerSecond * (float)num, value, DistanceIncreasePerSecond);
		}
	}

	private void AddHeadingInAngledDirectionMovement(ProjectileVisualization projectileVisualizationInstance)
	{
		projectileVisualizationInstance.gameObject.AddComponent<HeadingInAngledDirectionMovement>();
	}

	public float CalculateScaleModifier()
	{
		switch (SizeScaleSource)
		{
		case Enums.SizeScaleSource.FromProjectileSize:
			_scaleModifier = _weaponStats.TryGet(Enums.WeaponStatType.ProjectileSizePercentage, 1f);
			break;
		case Enums.SizeScaleSource.FromAoeSize:
			_scaleModifier = _weaponStats.TryGet(Enums.WeaponStatType.ExplosionSizePercentage, 1f);
			break;
		}
		return _scaleModifier;
	}

	internal void SetCustomStartPosition(Vector2 position)
	{
		ProjectileStartPosition = Enums.ProjectileStartPosition.OnCustomPosition;
		StartPositionLocation = position;
	}

	internal Vector3 GetStartPosition(Vector2 targetPosition)
	{
		switch (ProjectileStartPosition)
		{
		case Enums.ProjectileStartPosition.OnPlayer:
			return SingletonController<GameController>.Instance.PlayerPosition;
		case Enums.ProjectileStartPosition.OnTarget:
			return targetPosition;
		default:
			Debug.LogWarning(string.Format("Enum value {0} is not handled in {1}.{2}", ProjectileStartPosition, "WeaponAttack", "GetStartPosition"));
			return new Vector2(0f, 0f);
		}
	}

	internal Vector2 GetTargetPosition(Vector2 sourcePosition, Vector2 targetPosition, float range, float spreadOffsetDegrees)
	{
		switch (ProjectileMovement)
		{
		case Enums.ProjectileMovement.None:
			return targetPosition;
		case Enums.ProjectileMovement.HeadingTowardsTarget:
			return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
		case Enums.ProjectileMovement.Waving:
			return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
		case Enums.ProjectileMovement.MovingIntoAngleDirection:
			return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
		case Enums.ProjectileMovement.Boomerang:
			return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
		case Enums.ProjectileMovement.Thrown:
			return GetTargetPositionAtNormalRange(sourcePosition, targetPosition, spreadOffsetDegrees);
		case Enums.ProjectileMovement.MovingFacingPlayerDirection:
			return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
		case Enums.ProjectileMovement.MovingFacingPlayerAim:
			return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
		default:
			Debug.LogWarning(string.Format("Enum value {0} is not handled in {1}.{2}", ProjectileMovement, "WeaponAttack", "GetTargetPosition"));
			break;
		case Enums.ProjectileMovement.RotatingAroundStartPosition:
			break;
		}
		return sourcePosition;
	}

	private Vector2 GetTargetPositionAtNormalRange(Vector2 sourcePosition, Vector2 targetPosition, float spreadOffsetDegrees)
	{
		float range = Vector2.Distance(sourcePosition, targetPosition);
		return GetTargetAtMaxRange(sourcePosition, targetPosition, range, spreadOffsetDegrees);
	}

	private Vector2 GetTargetAtMaxRange(Vector2 sourcePosition, Vector2 targetPosition, float range, float spreadOffsetDegrees)
	{
		Vector2 offsetTargetPosition = GetOffsetTargetPosition(sourcePosition, targetPosition, spreadOffsetDegrees);
		Vector2 vector = offsetTargetPosition - sourcePosition;
		float num = Vector2.Distance(offsetTargetPosition, sourcePosition);
		Vector2 vector2 = range / num * vector;
		return offsetTargetPosition + vector2;
	}

	public static Vector2 GetOffsetTargetPosition(Vector2 sourcePosition, Vector2 originalTargetPosition, float offsetDegrees)
	{
		float x = originalTargetPosition.x - sourcePosition.x;
		float y = originalTargetPosition.y - sourcePosition.y;
		float num = Vector2.Distance(sourcePosition, originalTargetPosition);
		float f = (Mathf.Atan2(y, x) * 57.29578f + offsetDegrees) * (MathF.PI / 180f);
		float num2 = Mathf.Cos(f) * num;
		float num3 = Mathf.Sin(f) * num;
		return new Vector2(sourcePosition.x + num2, sourcePosition.y + num3);
	}

	internal Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition)
	{
		if (ProjectileMovement == Enums.ProjectileMovement.None)
		{
			return currentPosition;
		}
		Vector2 newPosition = GetNewPosition(currentPosition, targetPosition, _projectileSpeed * Time.deltaTime);
		if (!double.IsNaN(newPosition.x) && !double.IsNaN(newPosition.y))
		{
			return newPosition;
		}
		return currentPosition;
	}

	private Vector2 GetNewPosition(Vector2 currentPosition, Vector2 targetPosition, float maxMovementPerFrame)
	{
		if (_projectileMovement != null)
		{
			return _projectileMovement.GetNewPosition(currentPosition, targetPosition, maxMovementPerFrame);
		}
		return currentPosition;
	}

	internal Vector3 GetRotationAngle(Vector2 sourcePosition, Vector2 targetPosition, out float angle)
	{
		angle = 0f;
		switch (ProjectileRotation)
		{
		case Enums.ProjectileRotation.None:
			return new Vector3(0f, 0f, 0f);
		case Enums.ProjectileRotation.TowardsTheEnemy:
		{
			float x = sourcePosition.x - targetPosition.x;
			float y = sourcePosition.y - targetPosition.y;
			angle = Mathf.Atan2(y, x) * 57.29578f;
			if (double.IsNaN(angle))
			{
				return new Vector3(0f, 0f, 0f);
			}
			angle += RotationFix;
			return new Vector3(0f, 0f, angle);
		}
		case Enums.ProjectileRotation.StableAngle:
			angle = Angle + RotationFix;
			return new Vector3(0f, 0f, angle);
		case Enums.ProjectileRotation.RotatingConstantly:
			return new Vector3(0f, 0f, 0f);
		default:
			return new Vector3(0f, 0f, 0f);
		}
	}

	internal Vector2 GetRotatedOffset(float rotationAngleInDegrees, Enums.ProjectileStartPointFlip projectileStartPointFlip)
	{
		if (projectileStartPointFlip == Enums.ProjectileStartPointFlip.FlipWithPlayer)
		{
			return StartPositionOffset;
		}
		float f = (0f - rotationAngleInDegrees) * (MathF.PI / 180f);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		float x = num * StartPositionOffset.x + num2 * StartPositionOffset.y;
		float y = num2 * StartPositionOffset.x + num * StartPositionOffset.y;
		return new Vector2(x, y);
	}
}
