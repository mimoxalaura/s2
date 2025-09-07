using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Shop;
using BackpackSurvivors.UI.Stats;
using UnityEngine;

namespace BackpackSurvivors.Game.TrainingRoom;

public class TrainingRoomItemSelectionController : BaseModalUIController
{
	[SerializeField]
	private TrainingRoomItemSelectionUI _trainingRoomItemSelectionUI;

	[SerializeField]
	private Canvas _backdropCanvas;

	public override void CloseUI()
	{
		base.CloseUI();
		_trainingRoomItemSelectionUI.CloseUI();
		_backdropCanvas.gameObject.SetActive(value: false);
		SingletonController<StatController>.Instance.SetDisplayToggleButton(showButton: false);
		Object.FindObjectOfType<ModalUiController>().CloseAll();
		SingletonController<StatController>.Instance.CloseUI();
		SingletonController<StatController>.Instance.ToggleOpenWithSoloBackdrop(openWithSoloBackdrop: true);
		SingletonController<StatController>.Instance.ToggleOpenWithBuffsAndDebuffs(openWithBuffsAndDebuffs: true);
		SingletonController<InputController>.Instance.RevertToPreviousActionMap();
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.TrainingRoomItemSelection;
	}

	public override void OpenUI()
	{
		base.OpenUI();
		SingletonController<InputController>.Instance.SwitchToUIActionMap();
		SingletonController<StatController>.Instance.SetDisplayToggleButton(showButton: false);
		_trainingRoomItemSelectionUI.gameObject.SetActive(value: true);
		_trainingRoomItemSelectionUI.OpenUI();
		_backdropCanvas.gameObject.SetActive(value: true);
		Object.FindObjectOfType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.Backpack);
		SingletonController<StatController>.Instance.ToggleOpenWithSoloBackdrop(openWithSoloBackdrop: false);
		SingletonController<StatController>.Instance.ToggleOpenWithBuffsAndDebuffs(openWithBuffsAndDebuffs: false);
		SingletonController<StatController>.Instance.OpenUI();
		RegenerateAll();
	}

	public override bool CloseOnCancelInput()
	{
		return false;
	}

	internal void RegenerateAll()
	{
		_trainingRoomItemSelectionUI.ClearItems();
		_trainingRoomItemSelectionUI.InitWeapons();
		_trainingRoomItemSelectionUI.InitItems();
		_trainingRoomItemSelectionUI.InitBags();
	}

	public override bool AudioOnOpen()
	{
		return true;
	}

	public override bool AudioOnClose()
	{
		return true;
	}
}
