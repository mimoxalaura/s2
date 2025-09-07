using UnityEngine;

namespace BackpackSurvivors.UI.MainMenu;

public class MainMenuDemoButton : MonoBehaviour
{
	public void OpenNoticeButtonPressed()
	{
		Object.FindObjectOfType<WorkInProgressMessageUI>().OpenUI();
	}
}
