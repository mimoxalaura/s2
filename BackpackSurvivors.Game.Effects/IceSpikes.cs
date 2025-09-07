using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class IceSpikes : LingeringEffect
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Animator _animator;

	public override void Activate()
	{
		base.Activate();
		GetAnimator().SetBool("Active", value: true);
		GetAnimator().SetTrigger("Start");
	}

	private Animator GetAnimator()
	{
		if (_animator == null)
		{
			_animator = GetComponent<Animator>();
		}
		return _animator;
	}

	public override void End()
	{
		GetAnimator().SetBool("Active", value: false);
		GetAnimator().SetTrigger("Complete");
		Object.Destroy(base.gameObject, 1f);
	}

	public override void SetScale(float size)
	{
		EffectsHelper.Scale(_spriteRenderer.transform, size);
	}

	public override int GetSortingOrder()
	{
		return _spriteRenderer.sortingOrder;
	}

	public override void SetSortingOrder(int sortingOrder)
	{
		_spriteRenderer.sortingOrder = sortingOrder;
	}
}
