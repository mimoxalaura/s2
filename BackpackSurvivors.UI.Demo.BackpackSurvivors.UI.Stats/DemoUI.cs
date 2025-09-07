using BackpackSurvivors.Game.Core;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using UnityEngine;

namespace BackpackSurvivors.UI.Demo.BackpackSurvivors.UI.Stats;

public class DemoUI : ModalUI
{
	[SerializeField]
	private GameObject _blackBackdrop;

	[SerializeField]
	private Canvas _demoCanvas;

	public override void AfterCloseUI()
	{
		if (GameDatabase.IsDemo)
		{
			base.AfterCloseUI();
			base.gameObject.SetActive(value: false);
			_demoCanvas.enabled = false;
			_blackBackdrop.SetActive(value: false);
		}
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		if (GameDatabase.IsDemo)
		{
			_demoCanvas.gameObject.SetActive(value: true);
			_demoCanvas.enabled = true;
			_blackBackdrop.SetActive(value: true);
			base.OpenUI(openDirection);
		}
	}
}
