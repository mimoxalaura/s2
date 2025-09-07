using System.Collections;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using UnityEngine;

namespace BackpackSurvivors.UI.MainMenu;

public class WorkInProgressMessageUI : ModalUI
{
	[SerializeField]
	private Camera _camera;

	public void ShowUI()
	{
		StartCoroutine(OpenAfterDelay(2.5f));
	}

	private IEnumerator OpenAfterDelay(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);
		_camera.gameObject.SetActive(value: true);
		OpenUI();
	}

	public void CloseUI()
	{
		CloseUI(Enums.Modal.OpenDirection.Both);
	}

	public override void AfterCloseUI()
	{
		_camera.gameObject.SetActive(value: true);
	}
}
