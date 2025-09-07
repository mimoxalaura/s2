using UnityEngine;

namespace BackpackSurvivors.UI.GameplayFeedback;

public class DashElementPlayer : MonoBehaviour
{
	public bool IsReady;

	public int Index;

	internal void ToggleReadyState(bool isReady)
	{
		IsReady = isReady;
	}
}
