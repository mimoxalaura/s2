using System;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.MainMenu;
using BackpackSurvivors.UI.Stats;
using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.MainMenu;

public class MainMenuController : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _playerName;

	[SerializeField]
	private RawImage _playerImage;

	[SerializeField]
	private Transform _playerImageTarget;

	[SerializeField]
	private Transform _playerImageContainer;

	[SerializeField]
	private GameObject _container;

	[Header("Prefabs")]
	[SerializeField]
	private WorkInProgressMessageUI _workInProgressMessageUI;

	[Header("Player")]
	[SerializeField]
	private GameObject _character1;

	[SerializeField]
	private GameObject _character2;

	[SerializeField]
	private GameObject _character3;

	[SerializeField]
	private GameObject _character4;

	[Header("Scenes")]
	[SerializeField]
	private SceneReference _town;

	[SerializeField]
	private SceneReference _credits;

	[SerializeField]
	private Button _continueGameButton;

	[SerializeField]
	private Button _loadGameButton;

	private SaveSlotUIController _saveSlotUIController;

	private ModalUiController _modalUiController;

	private void Start()
	{
		SingletonController<InputController>.Instance.Clear();
		SingletonController<ReviveController>.Instance?.SetReviveCountUIVisibility(visible: false);
		_playerName.SetText(SingletonController<SteamController>.Instance.GetSteamName().ToUpper());
		try
		{
			_playerImage.texture = SingletonController<SteamController>.Instance.GetSteamAvatar(out var success);
			_playerImage.gameObject.SetActive(success);
			_playerImageContainer.gameObject.SetActive(success);
			_playerImage.color = new Color(255f, 255f, 255f, 0f);
			LeanTween.value(base.gameObject, delegate(float val)
			{
				_playerImage.color = new Color(255f, 255f, 255f, val);
			}, 0f, 1f, 2f);
		}
		catch (Exception message)
		{
			_playerImageContainer.gameObject.SetActive(value: false);
			_playerImageContainer.gameObject.SetActive(value: false);
			Debug.LogWarning(message);
		}
		SetupContinue();
		SetupLoadButton();
		RegisterControllers();
		RegisterEvents();
		Time.timeScale = 1f;
	}

	private void SetupLoadButton()
	{
		_loadGameButton.gameObject.SetActive(SingletonController<SaveGameController>.Instance.ActiveSaveGame != null && SingletonController<SaveGameController>.Instance.ActiveSaveGame.HasData());
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<InputController>.Instance, SwitchToUIActionMap);
	}

	private void SwitchToUIActionMap()
	{
		SingletonController<InputController>.Instance.SwitchToUIActionMap(storeCurrentInputMap: true, shouldPause: false);
	}

	private void RegisterControllers()
	{
		_saveSlotUIController = UnityEngine.Object.FindObjectOfType<SaveSlotUIController>();
		_modalUiController = UnityEngine.Object.FindObjectOfType<ModalUiController>();
	}

	private void SetupContinue()
	{
		_continueGameButton.gameObject.SetActive(SingletonController<SaveGameController>.Instance.ActiveSaveGame != null && SingletonController<SaveGameController>.Instance.ActiveSaveGame.HasData());
		if (SingletonController<SaveGameController>.Instance.ActiveSaveGame != null && SingletonController<SaveGameController>.Instance.ActiveSaveGame.CharacterExperienceState != null)
		{
			_character1.SetActive(SingletonController<SaveGameController>.Instance.ActiveSaveGame.CharacterExperienceState.ActiveCharacterId == 1);
			_character2.SetActive(SingletonController<SaveGameController>.Instance.ActiveSaveGame.CharacterExperienceState.ActiveCharacterId == 2);
			_character3.SetActive(SingletonController<SaveGameController>.Instance.ActiveSaveGame.CharacterExperienceState.ActiveCharacterId == 3);
			_character4.SetActive(SingletonController<SaveGameController>.Instance.ActiveSaveGame.CharacterExperienceState.ActiveCharacterId == 4);
		}
		else
		{
			_character1.SetActive(value: true);
		}
	}

	public void SetContinueButton(bool active)
	{
		_continueGameButton.gameObject.SetActive(active);
	}

	public void MainMenu_NewGame()
	{
		MoveMainMenuGameObjectsToLeft();
		_saveSlotUIController.SelectFirstSlot(shouldHaveData: false);
	}

	public void MainMenu_LoadGame()
	{
		MoveMainMenuGameObjectsToLeft();
		_saveSlotUIController.SelectFirstSlot(shouldHaveData: true);
	}

	public void MainMenu_ContinueGame()
	{
		SingletonController<SceneChangeController>.Instance.ChangeScene(_town.ScenePath, LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
	}

	public void MainMenu_BackToMain()
	{
		MoveMainMenuGameObjectsToRight();
		_saveSlotUIController.ResetSelected();
	}

	public void MainMenu_Settings()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Settings))
		{
			_modalUiController.OpenModalUI(Enums.ModalUITypes.Settings);
		}
	}

	public void MainMenu_Credits()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Credits))
		{
			SingletonController<SceneChangeController>.Instance.ChangeScene(_credits.ScenePath);
		}
	}

	public void MainMenu_Collection()
	{
		if (SingletonController<DEBUG_FeatureToggleController>.Instance.TryFeatureAndPopup(DEBUG_FeatureToggleController.Feature.Collection))
		{
			_modalUiController.OpenModalUI(Enums.ModalUITypes.Collection);
		}
	}

	public void MainMenu_Exit()
	{
		Application.Quit();
	}

	public void MainMenu_Socials_Twitter()
	{
		Application.OpenURL("https://x.com/CMGameDev");
	}

	public void MainMenu_Socials_Discord()
	{
		Application.OpenURL("https://discord.gg/xjDKSkGWhU");
	}

	public void MainMenu_Socials_Youtube()
	{
		Application.OpenURL("https://www.youtube.com/@CMGameStudio");
	}

	public void MainMenu_Socials_Steam()
	{
		Application.OpenURL("https://store.steampowered.com/app/2294780/Backpack_Survivors");
	}

	private void MoveMainMenuGameObjectsToLeft()
	{
		LeanTween.moveLocalX(_container, -1920f, 0.5f);
	}

	private void MoveMainMenuGameObjectsToRight()
	{
		LeanTween.moveLocalX(_container, 0f, 0.5f);
	}
}
