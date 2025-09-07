using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects.Core;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public static class EffectsHelper
{
	public static void SetLingeringEffectBaseValues(LingeringEffect lingeringEffect, Character source, Character target, Enums.LingeringEffectRotation lingeringEffectRotation)
	{
		Quaternion rotation = default(Quaternion);
		switch (lingeringEffectRotation)
		{
		case Enums.LingeringEffectRotation.Default:
			rotation = default(Quaternion);
			break;
		case Enums.LingeringEffectRotation.Random:
			rotation = new Quaternion(Random.rotation.x, Random.rotation.x, 0f, 0f);
			break;
		}
		lingeringEffect.SetRotation(rotation);
		int nextSortingOrder = SingletonController<LingeringEffectsController>.Instance.GetNextSortingOrder();
		lingeringEffect.SetSortingOrder(nextSortingOrder);
		lingeringEffect.SetScale(1f);
		lingeringEffect.SetDuration(5f);
		SingletonController<LingeringEffectsController>.Instance.AddLingeringEffect(lingeringEffect);
	}

	public static void DestroyLingeringEffect(LingeringEffect lingeringEffect)
	{
		SingletonController<LingeringEffectsController>.Instance.RemoveLingeringEffect(lingeringEffect);
	}

	public static float Scale(Transform transform, float size)
	{
		transform.LeanScaleX(transform.localScale.x * size, 0f);
		transform.LeanScaleY(transform.localScale.y * size, 0f);
		transform.LeanScaleZ(transform.localScale.z * size, 0f);
		return size;
	}

	public static float ScaleParticles(ParticleSystem particleSystem, float size)
	{
		ParticleSystem.ShapeModule shape = particleSystem.shape;
		shape.scale *= size;
		return Scale(particleSystem.transform, size);
	}

	internal static DebuffHandler CopyDebuffHandler(DebuffHandler item, CombatWeapon combatWeapon, Character enemyHit)
	{
		DebuffHandler debuffHandler = new DebuffHandler();
		debuffHandler.Init(item.DebuffSO, combatWeapon, enemyHit);
		return debuffHandler;
	}
}
