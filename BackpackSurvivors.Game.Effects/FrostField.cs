using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class FrostField : LingeringEffect
{
	[SerializeField]
	private SpriteMask _spriteMask;

	[SerializeField]
	private Sprite[] _masks;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Transform _particleSystem;

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
		Object.Destroy(base.gameObject, 6.5f);
	}

	public override void SetSortingOrder(int sortingOrder)
	{
		GetSpriteRenderer().sortingOrder = sortingOrder;
	}

	public override void SetScale(float size)
	{
		EffectsHelper.Scale(_spriteMask.transform, size);
		EffectsHelper.Scale(_particleSystem, size);
	}

	public override int GetSortingOrder()
	{
		return GetSpriteRenderer().sortingOrder;
	}

	private SpriteRenderer GetSpriteRenderer()
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
}
