using System.Collections;
using UnityEngine;

namespace BackpackSurvivors.Game.Player;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerDashVisualPrefab : MonoBehaviour
{
	[SerializeField]
	private float _fadeTime;

	private SpriteRenderer _spriteRenderer;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		LeanTween.value(_spriteRenderer.gameObject, delegate(float val)
		{
			_spriteRenderer.color = new Color(255f, 255f, 255f, val);
		}, 1f, 0f, _fadeTime);
		StartCoroutine(DestroyAferDelay());
	}

	private IEnumerator DestroyAferDelay()
	{
		yield return new WaitForSecondsRealtime(_fadeTime);
		Object.Destroy(base.gameObject);
	}

	internal void SetMaterial(Material material)
	{
		_spriteRenderer.material = material;
	}
}
