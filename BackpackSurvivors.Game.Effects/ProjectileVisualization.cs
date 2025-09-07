using System;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class ProjectileVisualization : MonoBehaviour
{
	internal delegate void TriggerOnTouchHandler(object sender, TriggerOnTouchEventArgs e);

	internal delegate void OnDestroyHandler(object sender, EventArgs e);

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private SpriteRenderer[] _additionalSpriteRenderers;

	[SerializeField]
	private ProjectileVisualizationFollower _projectileVisualizationFollower;

	internal event TriggerOnTouchHandler OnTriggerOnTouch;

	internal event OnDestroyHandler OnDestroyEvent;

	private void Start()
	{
		_spriteRenderer.sortingOrder = UnityEngine.Random.Range(-5000, 5000);
		if (_projectileVisualizationFollower != null)
		{
			ProjectileVisualizationFollower projectileVisualizationFollower = UnityEngine.Object.Instantiate(_projectileVisualizationFollower, base.transform);
			projectileVisualizationFollower.transform.SetParent(null);
			projectileVisualizationFollower.Init(this);
		}
	}

	internal virtual void OverrideAwake()
	{
	}

	private void Awake()
	{
		OverrideAwake();
	}

	internal void RaiseOnTriggerOnTouch(Character character)
	{
		this.OnTriggerOnTouch?.Invoke(this, new TriggerOnTouchEventArgs(character));
	}

	internal void Rotate(float x, float y, float z)
	{
		_spriteRenderer.transform.Rotate(x, y, z);
	}

	internal void Scale(float scale)
	{
		Vector3 to = base.transform.localScale * scale;
		base.gameObject.transform.LeanScale(to, 0f);
	}

	internal void MultiplyLocalScale(Vector3 multiplicator)
	{
		Vector3 localScale = base.transform.localScale;
		LeanTweenExt.LeanScale(to: new Vector3(localScale.x * multiplicator.x, localScale.y * multiplicator.y, localScale.z * multiplicator.z), transform: base.gameObject.transform, time: 0f);
	}

	internal void SetMaterial(Enums.DamageType damageType)
	{
		_spriteRenderer.material = MaterialHelper.GetAttackElementTypeMaterial(damageType);
		SpriteRenderer[] additionalSpriteRenderers = _additionalSpriteRenderers;
		for (int i = 0; i < additionalSpriteRenderers.Length; i++)
		{
			additionalSpriteRenderers[i].material = MaterialHelper.GetAttackElementTypeMaterial(damageType);
		}
	}

	internal void SpawnAnimation()
	{
		if (!(_animator == null) && _animator.parameters.Any((AnimatorControllerParameter x) => x.name == "Going") && _animator.parameters.Any((AnimatorControllerParameter x) => x.name == "Spawn"))
		{
			_animator.SetBool("Going", value: true);
			_animator.SetTrigger("Spawn");
		}
	}

	internal void HitAnimation()
	{
		if (!(_animator == null) && _animator.parameters.Any((AnimatorControllerParameter x) => x.name == "Going") && _animator.parameters.Any((AnimatorControllerParameter x) => x.name == "OnHit"))
		{
			_animator.SetBool("Going", value: false);
			_animator.SetTrigger("OnHit");
		}
	}

	internal void DestroyAnimation()
	{
		if (!(_animator == null) && !(_animator.runtimeAnimatorController == null) && _animator.parameters.Any((AnimatorControllerParameter x) => x.name == "Destroying"))
		{
			_animator.SetBool("Destroying", value: true);
		}
	}

	internal void SetContinuousRotation(float rotationsPerSecond)
	{
		float num = 10f;
		float z = 360f * rotationsPerSecond * num;
		LeanTween.rotate(base.gameObject, new Vector3(0f, 0f, z), num);
	}

	internal virtual void OverrideDestroy()
	{
	}

	private void OnDestroy()
	{
		this.OnDestroyEvent?.Invoke(this, new EventArgs());
		OverrideDestroy();
		LeanTween.cancel(base.gameObject);
	}

	internal virtual void StartAttackLogic()
	{
	}

	internal virtual void InitTriggers(Enums.CharacterType characterTypeForTriggerOnTouch, Character source, Vector2 targetPosition)
	{
	}

	internal void StopRotation()
	{
		LeanTween.cancel(base.gameObject);
	}
}
