using System.Collections;
using UnityEngine;

public class WarningLineAreaOfEffect : MonoBehaviour
{
	private float _defaultScale = 0.5f;

	private float _spawningDuration = 0.1f;

	private float _initialSize = 2f;

	[SerializeField]
	private SpriteRenderer _initialSpriteRenderer;

	[SerializeField]
	private SpriteRenderer _increasingSpriteRenderer;

	public void Init(Vector3 startPosition, float width, Vector3 targetPosition, float warningDuration)
	{
		_increasingSpriteRenderer.transform.localScale = new Vector3(1f, 0f, 1f);
		base.transform.position = startPosition;
		Quaternion.LookRotation(Vector3.Cross(startPosition, targetPosition));
		Vector2.SignedAngle(startPosition, targetPosition);
		float num = Vector3.Distance(startPosition, targetPosition);
		base.transform.localScale = new Vector3(1f, width, 1f);
		base.transform.rotation = Quaternion.FromToRotation(startPosition, targetPosition);
		LeanTween.value(_initialSpriteRenderer.gameObject, UpdateInitialSize, 0f, num, _spawningDuration);
		_increasingSpriteRenderer.size = new Vector2(num, _initialSize);
		LeanTween.scaleY(_increasingSpriteRenderer.gameObject, _defaultScale, warningDuration).setDelay(_spawningDuration);
		StartCoroutine(DestroyThis(warningDuration));
	}

	private void UpdateInitialSize(float size)
	{
		_initialSpriteRenderer.size = new Vector2(size, _initialSize);
	}

	private IEnumerator DestroyThis(float delay)
	{
		yield return new WaitForSeconds(delay);
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		CancelLeanTween();
	}

	private void CancelLeanTween()
	{
		LeanTween.cancel(base.gameObject);
		LeanTween.cancel(_initialSpriteRenderer.gameObject);
		LeanTween.cancel(_increasingSpriteRenderer.gameObject);
	}
}
