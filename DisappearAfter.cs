using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal class DisappearAfter : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private Image _image;

	[SerializeField]
	private float _alphaReduction;

	[SerializeField]
	private float _alphaReductionDelay;

	[SerializeField]
	private float _delayBeforeStart;

	private void Start()
	{
		if (_spriteRenderer != null)
		{
			StartCoroutine(StartHidingSpriteRenderer());
		}
	}

	private IEnumerator StartHidingSpriteRenderer()
	{
		yield return new WaitForSeconds(_delayBeforeStart);
		Color baseColor = _spriteRenderer.color;
		float alpha = baseColor.a;
		while (_spriteRenderer.color.a > 0f)
		{
			alpha -= _alphaReduction;
			_spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
			yield return new WaitForSeconds(_alphaReductionDelay);
		}
		Object.Destroy(base.gameObject);
	}
}
