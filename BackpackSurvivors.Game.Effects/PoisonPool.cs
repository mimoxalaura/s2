using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class PoisonPool : LingeringEffect
{
	[SerializeField]
	private SpriteMask _spriteMask;

	[SerializeField]
	private Sprite[] _masks;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Animator _animator;

	public override void Activate()
	{
		base.Activate();
		int num = Random.Range(0, _masks.Length);
		_spriteMask.sprite = _masks[num];
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
		_spriteRenderer.sortingOrder = sortingOrder;
	}

	public override void SetScale(float size)
	{
		EffectsHelper.Scale(_spriteMask.transform, size);
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
