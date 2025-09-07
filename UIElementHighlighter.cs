using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIElementHighlighter : MonoBehaviour
{
	[SerializeField]
	private Image LeftTop;

	[SerializeField]
	private Transform LeftTopTarget;

	[SerializeField]
	private Image RightTop;

	[SerializeField]
	private Transform RightTopTarget;

	[SerializeField]
	private Image LeftBottom;

	[SerializeField]
	private Transform LeftBottomTarget;

	[SerializeField]
	private Image RightBottom;

	[SerializeField]
	private Transform RightBottomTarget;

	private Vector2 LeftTopTargetVector = new Vector2(16f, -16f);

	private Vector2 RightTopTargetVector = new Vector2(-16f, -16f);

	private Vector2 LeftBottomTargetVector = new Vector2(16f, 16f);

	private Vector2 RightBottomTargetVector = new Vector2(-16f, 16f);

	[SerializeField]
	private float _animationDuration = 1f;

	private bool isActive = true;

	private void Start()
	{
		Hide();
		LeftTopTarget.GetComponent<Image>().enabled = false;
		RightTopTarget.GetComponent<Image>().enabled = false;
		LeftBottomTarget.GetComponent<Image>().enabled = false;
		RightBottomTarget.GetComponent<Image>().enabled = false;
	}

	public void Move(Vector3 position, Vector2 size)
	{
		((RectTransform)base.transform).sizeDelta = new Vector2(size.x, size.y);
		((RectTransform)base.transform).position = new Vector2(position.x, position.y);
	}

	private void StartAnimation(Image image, Vector2 target, Vector2 targetModifier)
	{
		StartCoroutine(AnimateImage(image, target + targetModifier, target));
	}

	private IEnumerator AnimateImage(Image image, Vector2 target, Vector2 originPosition)
	{
		bool isTravellingForward = true;
		while (isActive)
		{
			if (isTravellingForward)
			{
				LeanTween.move(image.gameObject, target, _animationDuration).setEaseInOutCirc().setIgnoreTimeScale(useUnScaledTime: true)
					.setOnComplete((Action)delegate
					{
						isTravellingForward = false;
					});
			}
			else
			{
				LeanTween.move(image.gameObject, originPosition, _animationDuration).setEaseInOutCirc().setIgnoreTimeScale(useUnScaledTime: true)
					.setOnComplete((Action)delegate
					{
						isTravellingForward = true;
					});
			}
			float startTime = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup - startTime < 1f)
			{
				yield return null;
			}
		}
	}

	internal void Hide()
	{
		LeftTop.enabled = false;
		RightTop.enabled = false;
		LeftBottom.enabled = false;
		RightBottom.enabled = false;
		LeanTween.cancel(LeftTop.gameObject);
		LeanTween.cancel(RightTop.gameObject);
		LeanTween.cancel(LeftBottom.gameObject);
		LeanTween.cancel(RightBottom.gameObject);
		StopAllCoroutines();
		LeftTop.transform.position = LeftTopTarget.position;
		RightTop.transform.position = RightTopTarget.position;
		LeftBottom.transform.position = LeftBottomTarget.position;
		RightBottom.transform.position = RightBottomTarget.position;
	}

	internal void Show(Canvas canvas)
	{
		base.transform.SetParent(canvas.transform);
		LeftTop.enabled = true;
		RightTop.enabled = true;
		LeftBottom.enabled = true;
		RightBottom.enabled = true;
		LeftTop.transform.position = LeftTopTarget.position;
		RightTop.transform.position = RightTopTarget.position;
		LeftBottom.transform.position = LeftBottomTarget.position;
		RightBottom.transform.position = RightBottomTarget.position;
		LeanTween.cancel(LeftTop.gameObject);
		LeanTween.cancel(RightTop.gameObject);
		LeanTween.cancel(LeftBottom.gameObject);
		LeanTween.cancel(RightBottom.gameObject);
		StartAnimation(LeftTop, LeftTopTarget.position, LeftTopTargetVector);
		StartAnimation(RightTop, RightTopTarget.position, RightTopTargetVector);
		StartAnimation(LeftBottom, LeftBottomTarget.position, LeftBottomTargetVector);
		StartAnimation(RightBottom, RightBottomTarget.position, RightBottomTargetVector);
	}
}
