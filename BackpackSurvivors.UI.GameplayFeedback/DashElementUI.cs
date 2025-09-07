using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class DashElementUI : MonoBehaviour
{
	[SerializeField]
	private GameObject _readyOverlay;

	public bool IsReady;

	public int Index;

	internal void ToggleReadyState(bool isReady)
	{
		IsReady = isReady;
		_readyOverlay.SetActive(isReady);
	}
}
