using System.Collections;
using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback.Currency;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LostOrReceivedCurrencyAnimation : MonoBehaviour
{
	private TextMeshProUGUI _addedOrRemovedMoneyText;

	private float offset = 40f;

	public void Init(int changedAmount, GameObject currentCurrencyValue)
	{
		_addedOrRemovedMoneyText = GetComponent<TextMeshProUGUI>();
		if (base.isActiveAndEnabled)
		{
			if (changedAmount > 0)
			{
				LeanTween.cancel(currentCurrencyValue);
				_addedOrRemovedMoneyText.color = Color.yellow;
				_addedOrRemovedMoneyText.SetText($"+{changedAmount}");
				Vector3 localPosition = base.gameObject.transform.localPosition;
				base.gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y - offset, localPosition.z);
				LeanTween.cancel(base.gameObject);
				LeanTween.moveLocal(base.gameObject, localPosition, 0.3f).setScale(1f).setEase(LeanTweenType.easeOutQuad)
					.setIgnoreTimeScale(useUnScaledTime: true);
			}
			else if (changedAmount < 0)
			{
				LeanTween.cancel(currentCurrencyValue);
				LeanTween.scale(currentCurrencyValue, new Vector3(0.8f, 0.8f, 0.8f), 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
				_addedOrRemovedMoneyText.color = Color.red;
				_addedOrRemovedMoneyText.SetText($"{changedAmount}");
				Vector3 to = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - offset, base.transform.localPosition.z);
				LeanTween.cancel(base.gameObject);
				LeanTween.moveLocal(base.gameObject, to, 0.3f).setScale(1f).setEase(LeanTweenType.easeOutQuad)
					.setIgnoreTimeScale(useUnScaledTime: true);
			}
			LeanTween.scale(currentCurrencyValue, new Vector3(1f, 1f, 1f), 0.2f).setDelay(0.2f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.scale(base.gameObject, new Vector3(0f, 0f, 0f), 0.2f).setDelay(0.5f).setIgnoreTimeScale(useUnScaledTime: true);
		}
		StartCoroutine(DoDestroy(2f));
	}

	private IEnumerator DoDestroy(float duration)
	{
		yield return new WaitForSecondsRealtime(duration);
		LeanTween.cancel(base.gameObject);
		base.gameObject.SetActive(value: true);
		Object.Destroy(base.gameObject);
	}
}
