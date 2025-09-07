using UnityEngine;

public class FlipWithSpriteFlipX : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	private bool _previousFlipState;

	private void Start()
	{
		_previousFlipState = _spriteRenderer.flipX;
	}

	private void Update()
	{
		if (_previousFlipState != _spriteRenderer.flipX)
		{
			_previousFlipState = _spriteRenderer.flipX;
			base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
