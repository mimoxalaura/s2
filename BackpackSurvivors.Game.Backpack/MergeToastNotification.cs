using System.Collections;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using QFSW.QC.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.Backpack;

internal class MergeToastNotification : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private Image _backdrop;

	private void UpdateCannotAffordTextAlpha(float alpha)
	{
		_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alpha);
		_backdrop.color = new Color(_backdrop.color.r, _backdrop.color.g, _backdrop.color.b, Mathf.Clamp(alpha, 0f, 0.7f));
	}

	internal void UpdatePosition(Vector2 targetPosition)
	{
		base.transform.localPosition = targetPosition;
		ResetZIndex();
	}

	internal void ResetZIndex()
	{
		StartCoroutine(ResetScaleAndZIndexAsync());
	}

	private IEnumerator ResetScaleAndZIndexAsync()
	{
		yield return new WaitFrame();
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, 0f);
	}

	internal void UpdateTextByMergeResult(BaseItemSO mergeResultBaseItemSO, int amountNeeded, int amountCurrent)
	{
		_text.color = new Color(255f, 255f, 255f, 0f);
		int num = Mathf.Clamp(amountCurrent, 0, amountNeeded);
		string empty = string.Empty;
		string text = string.Empty;
		empty = ((num != amountNeeded) ? ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase) : ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase));
		switch (mergeResultBaseItemSO.ItemType)
		{
		case Enums.PlaceableType.Weapon:
			text = "Weapon";
			break;
		case Enums.PlaceableType.Item:
			text = "Item";
			break;
		}
		_text.SetText($"<sprite name=\"{text}\"> <color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase)}> <b>{mergeResultBaseItemSO.Name}</b></color> <color={empty}>({num}/{amountNeeded})</color>");
	}

	internal void UpdateNoPlaceForMergeResultText()
	{
		_text.SetText("<color=#D20060>No space to place merge result</color>");
	}

	internal void UpdateMergingBlockedText()
	{
		_text.SetText("<color=#D20060>Merging Locked</color>");
	}

	internal void Display()
	{
		AnimateToastText();
	}

	private void AnimateToastText()
	{
		LeanTween.cancel(base.gameObject);
		float num = 0.3f;
		float num2 = 1f;
		float num3 = 0f;
		float num4 = 1f;
		LeanTween.scale(base.gameObject, Vector3.one, num).setEaseInOutElastic().setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(base.gameObject, UpdateCannotAffordTextAlpha, num3, num4, num).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(base.gameObject, UpdateCannotAffordTextAlpha, num4, num3, num).setDelay(num + num2).setIgnoreTimeScale(useUnScaledTime: true);
		StartCoroutine(DestroyAfterDelay(num2 * 2f));
	}

	private IEnumerator DestroyAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
		StopAllCoroutines();
	}
}
