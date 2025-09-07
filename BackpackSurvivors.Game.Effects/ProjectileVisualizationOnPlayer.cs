using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class ProjectileVisualizationOnPlayer : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	internal float RotationFix;

	[SerializeField]
	private bool _rotateXBasedOnSource;

	[Header("Delay")]
	[SerializeField]
	private GameObject[] _activateAfterDelay;

	[SerializeField]
	private float _delayBeforeActivation;

	private Projectile _projectile;

	public Transform TransformToRotate => _spriteRenderer?.transform;

	private void Start()
	{
	}

	private IEnumerator ActivateAfterDelayAsync(Projectile projectile)
	{
		yield return new WaitForSeconds(projectile.PlayerVisualizationSpawnDelay);
		GameObject[] activateAfterDelay = _activateAfterDelay;
		for (int i = 0; i < activateAfterDelay.Length; i++)
		{
			activateAfterDelay[i].SetActive(value: true);
		}
	}

	private IEnumerator DestroyAfterDelayAsync(GameObject parent)
	{
		DestroyAnimation();
		yield return new WaitForSeconds(_projectile.PlayerVisualizationDestructionDelay);
		Object.Destroy(parent);
	}

	internal void Rotate(float x, float y, float z)
	{
		if (!(_spriteRenderer == null))
		{
			_spriteRenderer.transform.Rotate(x, y, z);
		}
	}

	internal void Scale(float scale)
	{
		Vector3 to = base.transform.localScale * (1f + scale);
		base.gameObject.transform.LeanScale(to, 0f);
	}

	internal void SetMaterial(Enums.DamageType damageType)
	{
		if (!(_spriteRenderer == null))
		{
			_spriteRenderer.material = MaterialHelper.GetAttackElementTypeMaterial(damageType);
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

	internal void DestroyAnimation()
	{
		if (!(_animator == null) && _animator.parameters.Any((AnimatorControllerParameter x) => x.name == "Destroying"))
		{
			_animator.SetBool("Destroying", value: true);
		}
	}

	internal void Init(Character source, Character target, Projectile projectile)
	{
		bool flag = false;
		_projectile = projectile;
		switch (projectile.PlayerVisualizationFlip)
		{
		case Enums.ProjectileFlip.FlipXBasedOnTargetPosition:
			flag = source.transform.localPosition.x < target.transform.localPosition.x;
			if (!flag)
			{
				base.transform.localPosition = projectile.PlayerVisualizationStartPositionOffset;
			}
			else
			{
				base.transform.localPosition = new Vector2(projectile.PlayerVisualizationStartPositionOffset.x * -1f, projectile.PlayerVisualizationStartPositionOffset.y);
			}
			if (!(_spriteRenderer == null))
			{
				_spriteRenderer.flipX = flag;
			}
			break;
		case Enums.ProjectileFlip.FlipXBasedOnPlayerDirection:
			flag = !source.IsRotatedToRight();
			if (!flag)
			{
				base.transform.localPosition = projectile.PlayerVisualizationStartPositionOffset;
			}
			else
			{
				base.transform.localPosition = new Vector2(projectile.PlayerVisualizationStartPositionOffset.x * -1f, projectile.PlayerVisualizationStartPositionOffset.y);
			}
			if (!(_spriteRenderer == null))
			{
				_spriteRenderer.flipX = flag;
			}
			break;
		case Enums.ProjectileFlip.None:
			break;
		}
	}

	internal void InitiateDestruction(GameObject parentObject)
	{
		StartCoroutine(DestroyAfterDelayAsync(parentObject));
	}
}
