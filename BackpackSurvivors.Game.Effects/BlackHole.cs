using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BlackHole : LingeringEffect
{
	private SpriteRenderer _spriteRenderer;

	private Animator _animator;

	public override void Activate()
	{
		base.Activate();
		GetAnimator().SetBool("Active", value: true);
		GetAnimator().SetTrigger("Start");
	}

	public override void End()
	{
		GetAnimator().SetBool("Active", value: false);
		GetAnimator().SetTrigger("Complete");
		Object.Destroy(base.gameObject, GetAnimator().GetCurrentAnimatorStateInfo(0).length);
	}

	public override void SetSortingOrder(int sortingOrder)
	{
		GetSpriteRenderer().sortingOrder = sortingOrder;
	}

	public override void SetScale(float size)
	{
		base.SetScale(size);
	}

	public SpriteRenderer GetSpriteRenderer()
	{
		if (_spriteRenderer == null)
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
		return _spriteRenderer;
	}

	private Animator GetAnimator()
	{
		if (_animator == null)
		{
			_animator = GetComponent<Animator>();
		}
		return _animator;
	}

	public override int GetSortingOrder()
	{
		return _spriteRenderer.sortingOrder;
	}
}
