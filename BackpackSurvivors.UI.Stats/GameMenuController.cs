using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Feedback;
using BackpackSurvivors.UI.Shared;
using QFSW.QC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.UI.Stats;

public class GameMenuController : BaseModalUIController
{
	[SerializeField]
	private List<Enums.GameMenuButtonType> _gameMenuButtonTypes;

	[SerializeField]
	private GameMenuUI _gameMenuUI;

	private bool _toggled;

	private ModalUiController _modalUiController;

	private bool _preventOpeningSettings;

	private void Start()
	{
		foreach (Enums.GameMenuButtonType gameMenuButtonType in _gameMenuButtonTypes)
		{
			if (gameMenuButtonType != Enums.GameMenuButtonType.Feedback || !(UnityEngine.Object.FindObjectOfType<FeedbackUI>() == null))
			{
				_gameMenuUI.CreateButton(gameMenuButtonType);
			}
		}
		_gameMenuUI.OnMenuButtonClicked += _gameMenuUI_OnMenuButtonClicked;
		_gameMenuUI.OnAfterCloseUI += _gameMenuUI_OnAfterCloseUI;
		_modalUiController = UnityEngine.Object.FindAnyObjectByType<ModalUiController>();
	}

	private void _gameMenuUI_OnAfterCloseUI()
	{
		SetCamerasEnabled(enabled: false);
		SingletonController<BackpackController>.Instance.SetCamerasEnabled(enabled: false);
	}

	internal void ResetPreventOpeningSettings()
	{
		_preventOpeningSettings = false;
	}

	private void _gameMenuUI_OnMenuButtonClicked(object sender, OnMenuButtonClickedHandlerEventArgs e)
	{
		switch (e.GameMenuButtonType)
		{
		case Enums.GameMenuButtonType.Resume:
			_modalUiController.CloseAll();
			break;
		case Enums.GameMenuButtonType.Character:
			_modalUiController.OpenModalUI(Enums.ModalUITypes.Stats);
			break;
		case Enums.GameMenuButtonType.Settings:
			if (!_preventOpeningSettings && SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Settings))
			{
				_modalUiController.OpenModalUI(Enums.ModalUITypes.Settings);
			}
			break;
		case Enums.GameMenuButtonType.Collection:
			if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Collection))
			{
				_modalUiController.OpenModalUI(Enums.ModalUITypes.Collection);
			}
			break;
		case Enums.GameMenuButtonType.BackToTown:
			SingletonController<GenericPopupController>.Instance.CreatePopup(Enums.GenericPopupLocation.MiddleOfScreen, "Return to town?", "Are you sure you want to return to the town? You will lose all adventure progression.", Enums.GenericPopupButtons.No, Enums.GenericPopupButtons.No, closeOnCancel: true, null);
			SingletonController<GenericPopupController>.Instance.ClearEvents();
			SingletonController<GenericPopupController>.Instance.OnPopupButtonYesClicked += BackToTown_OnPopupButtonYesClicked;
			SingletonController<GenericPopupController>.Instance.OnPopupButtonNoClicked += BackToTown_OnPopupButtonNoClicked;
			SingletonController<GenericPopupController>.Instance.ShowPopup();
			break;
		case Enums.GameMenuButtonType.ExitGame:
			SingletonController<GenericPopupController>.Instance.CreatePopup(Enums.GenericPopupLocation.MiddleOfScreen, "Exit to Main Menu?", "Are you sure you want to return to the main menu?", Enums.GenericPopupButtons.No, Enums.GenericPopupButtons.No, closeOnCancel: true, null);
			SingletonController<GenericPopupController>.Instance.ClearEvents();
			SingletonController<GenericPopupController>.Instance.OnPopupButtonYesClicked += ExitGame_OnPopupButtonYesClicked;
			SingletonController<GenericPopupController>.Instance.OnPopupButtonNoClicked += ExitGame_OnPopupButtonNoClicked;
			SingletonController<GenericPopupController>.Instance.ShowPopup();
			break;
		case Enums.GameMenuButtonType.Feedback:
		{
			FeedbackUI feedbackUI = UnityEngine.Object.FindObjectOfType<FeedbackUI>();
			if (feedbackUI != null)
			{
				feedbackUI.OpenFeedbackUI();
			}
			break;
		}
		case Enums.GameMenuButtonType.QuitGame:
			SingletonController<GenericPopupController>.Instance.CreatePopup(Enums.GenericPopupLocation.MiddleOfScreen, "Quit game?", "Are you sure you want to exit the game?", Enums.GenericPopupButtons.No, Enums.GenericPopupButtons.No, closeOnCancel: true, null);
			SingletonController<GenericPopupController>.Instance.ClearEvents();
			SingletonController<GenericPopupController>.Instance.OnPopupButtonYesClicked += QuitGame_OnPopupButtonYesClicked;
			SingletonController<GenericPopupController>.Instance.OnPopupButtonNoClicked += QuitGame_OnPopupButtonNoClicked;
			SingletonController<GenericPopupController>.Instance.ShowPopup();
			break;
		}
	}

	private void BackToTown_OnPopupButtonNoClicked(object sender, EventArgs e)
	{
		SingletonController<GenericPopupController>.Instance.ClearEvents();
		SingletonController<GenericPopupController>.Instance.ClosePopup();
	}

	private void BackToTown_OnPopupButtonYesClicked(object sender, EventArgs e)
	{
		SingletonController<SaveGameController>.Instance.SaveProgression();
		_preventOpeningSettings = true;
		_gameMenuUI.CloseUI();
		SingletonController<AdventureCompletedController>.Instance.CloseAllUIs();
		SingletonController<GameController>.Instance.ClearControllersOfAdventureState();
		SingletonController<GameController>.Instance.ExitingFromPortal = true;
		SingletonController<SceneChangeController>.Instance.ChangeScene(GameDatabaseHelper.GetSceneFromType(Enums.SceneType.Town), LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
	}

	private void ExitGame_OnPopupButtonNoClicked(object sender, EventArgs e)
	{
		SingletonController<GenericPopupController>.Instance.ClearEvents();
		SingletonController<GenericPopupController>.Instance.ClosePopup();
	}

	private void ExitGame_OnPopupButtonYesClicked(object sender, EventArgs e)
	{
		SingletonController<SaveGameController>.Instance.SaveProgression();
		_preventOpeningSettings = true;
		_gameMenuUI.CloseUI();
		SingletonController<AdventureCompletedController>.Instance.CloseAllUIs();
		SingletonController<GameController>.Instance.ClearControllersOfAdventureState();
		SingletonController<SceneChangeController>.Instance.ChangeScene(GameDatabaseHelper.GetSceneFromType(Enums.SceneType.MainMenu));
	}

	private void QuitGame_OnPopupButtonNoClicked(object sender, EventArgs e)
	{
		SingletonController<GenericPopupController>.Instance.ClearEvents();
		SingletonController<GenericPopupController>.Instance.ClosePopup();
	}

	private void QuitGame_OnPopupButtonYesClicked(object sender, EventArgs e)
	{
		SingletonController<SaveGameController>.Instance.SaveProgression();
		Application.Quit();
	}

	[Command("menu.open", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void OpenUI()
	{
		if (!SingletonController<TutorialController>.Instance.IsRunningTutorial)
		{
			base.OpenUI();
			if (SingletonController<GameController>.Instance.CanOpenMenu)
			{
				SingletonController<GameController>.Instance.SetGamePaused(gamePaused: true);
				_gameMenuUI.OpenUI();
			}
		}
	}

	[Command("menu.close", Platform.AllPlatforms, MonoTargetType.Single)]
	public override void CloseUI()
	{
		if (SingletonController<GameController>.Instance.CanOpenMenu)
		{
			SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false);
			_gameMenuUI.CloseUI();
		}
	}

	public override Enums.ModalUITypes GetModalUIType()
	{
		return Enums.ModalUITypes.GameMenu;
	}

	public override bool CloseOnCancelInput()
	{
		return true;
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
