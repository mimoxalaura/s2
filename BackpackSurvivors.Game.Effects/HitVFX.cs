using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Effects;

internal class HitVFX : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Sprite[] _hitVFXSprites;

	private void Awake()
	{
		_spriteRenderer.enabled = false;
	}

	internal void ShowHit()
	{
		StopAllCoroutines();
		StartCoroutine(ShowHitAsync());
	}

	private IEnumerator ShowHitAsync()
	{
		_spriteRenderer.enabled = true;
		for (int i = 0; i < _hitVFXSprites.Length; i++)
		{
			_spriteRenderer.sprite = _hitVFXSprites[i];
			yield return new WaitForSeconds(0.1f);
		}
		_spriteRenderer.enabled = false;
	}
}
