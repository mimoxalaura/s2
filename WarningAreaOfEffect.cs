using System.Collections;
using UnityEngine;

public class WarningAreaOfEffect : MonoBehaviour
{
	private float _growingScalerPerOne = 5.5f;

	private float spawningDelay = 0.1f;

	private float radiusRescaler = 1f;

	[SerializeField]
	private Transform _initialSpriteRenderer;

	[SerializeField]
	private Transform _increasingSpriteRenderer;

	[SerializeField]
	private Transform _decreasingSpriteRenderer;

	public void Init(Vector3 position, float radius, float warningDuration)
	{
		base.transform.position = position;
		radius *= radiusRescaler;
		Vector3 vector = new Vector3(radius, radius, radius);
		float num = warningDuration - spawningDelay;
		num = Mathf.Clamp(num, 0.1f, num);
		_initialSpriteRenderer.transform.localScale = new Vector3(0f, 0f, 0f);
		LeanTween.scale(_initialSpriteRenderer.gameObject, vector, spawningDelay);
		float num2 = radius * 2f;
		_decreasingSpriteRenderer.transform.localScale = new Vector3(num2, num2, num2);
		float num3 = 0f;
		_increasingSpriteRenderer.transform.localScale = new Vector3(num3, num3, num3);
		float to = 360f * warningDuration;
		LeanTween.rotateZ(_initialSpriteRenderer.gameObject, to, warningDuration);
		float to2 = -360f * warningDuration;
		LeanTween.rotateZ(_decreasingSpriteRenderer.gameObject, to2, warningDuration);
		LeanTween.scale(_decreasingSpriteRenderer.gameObject, vector, num).setDelay(spawningDelay);
		Vector3 to3 = vector * _growingScalerPerOne;
		LeanTween.scale(_increasingSpriteRenderer.gameObject, to3, num).setDelay(spawningDelay);
		StartCoroutine(DestroyThis(warningDuration));
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
		LeanTween.cancel(_initialSpriteRenderer.gameObject);
		LeanTween.cancel(_increasingSpriteRenderer.gameObject);
		LeanTween.cancel(_decreasingSpriteRenderer.gameObject);
	}
}
