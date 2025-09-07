using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class DashElementUINew : MonoBehaviour
{
	[SerializeField]
	private GameObject _readyOverlay;

	[SerializeField]
	private Image _fillImage;

	public bool IsReady;

	public int Index;

	internal void ToggleReadyState(bool isReady)
	{
		IsReady = isReady;
		_readyOverlay.SetActive(isReady);
	}

	internal void SetProgress(float currentCooldown, float maxCooldown)
	{
		float num = maxCooldown - currentCooldown;
		_fillImage.fillAmount = num / maxCooldown;
	}

	internal void SetProgress(float percentage)
	{
		_fillImage.fillAmount = percentage;
	}
}
