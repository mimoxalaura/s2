using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

public class Fire : LingeringEffect
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	public override void Activate()
	{
		base.Activate();
	}

	public override void SetSortingOrder(int sortingOrder)
	{
		_spriteRenderer.sortingOrder = sortingOrder;
	}

	public override void End()
	{
		Object.Destroy(base.gameObject, 0f);
	}

	public override int GetSortingOrder()
	{
		return _spriteRenderer.sortingOrder;
	}
}
