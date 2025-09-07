using System.Collections;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Talents;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.GameplayFeedback;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

public class TownController : MonoBehaviour
{
	[SerializeField]
	private Material _lockedMaterial;

	[SerializeField]
	private Material _unlockedMaterial;

	[SerializeField]
	private UnlockableInTown[] _unlockableTownBuildings;

	[SerializeField]
	private CinemachineSwitcher _cinemachineSwitcher;

	[Header("Tutorial")]
	[SerializeField]
	private BackpackSurvivors.Game.Player.Player _player;

	[SerializeField]
	private Transform _spawnPointNewGame;

	[SerializeField]
	private Transform _spawnPointTrainingRoom;

	[SerializeField]
	private Transform _spawnPointPortalRoom;

	[SerializeField]
	private Transform _spawnPointContinueGame;

	[SerializeField]
	private Transform _infrontOfGatePosition;

	[SerializeField]
	private Transform _centerTownPosition;

	[SerializeField]
	private Transform _infrontOfPortalPosition;

	[SerializeField]
	private Transform _infrontOfUnlockAltarPosition;

	[SerializeField]
	private Transform _insideUnlockAltarInteractionRangePosition;

	[SerializeField]
	private GameObject _fortressGate;

	[SerializeField]
	private GameObject _gameplayCanvas;

	[SerializeField]
	private GameObject _movementTip;

	[SerializeField]
	private Canvas _tutorialCanvas;

	[SerializeField]
	private TitanicSoulsTutorialController _titanicSoulsTutorialController;

	[SerializeField]
	private Sprite _titanSoulSprite;

	private bool _tutorialControllerInitialized;

	private bool _gameControllerInitialized;

	private bool runningTutorial;

	private void Start()
	{
		SingletonController<SceneChangeController>.Instance.AllowFading = true;
		SingletonController<SaveGameController>.Instance.LoadProgression();
		SingletonController<BackpackController>.Instance.CloseUI();
		SingletonController<InputController>.Instance.SwitchToIngameActionMap(storeCurrentInputMap: false);
		SingletonController<UnlocksController>.Instance.OnUnlockableUnlocked += UnlocksController_OnUnlockableUnlocked;
		StartCoroutine(SpawnPlayerAfterReady());
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharactersControllerInitialized);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<GameController>.Instance, RegisterGameControllerInitialized);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<TutorialController>.Instance, RegisterTutorialControllerInitialized);
		GetComponent<MaterialPropertyController>().SetEnabled(enabled: false);
	}

	private void RegisterCharactersControllerInitialized()
	{
		SingletonController<UnlocksController>.Instance.SetUnlock(Enums.Unlockable.ExtraCharacter0);
		SingletonController<CharactersController>.Instance.SwitchCharacter(SingletonController<SaveGameController>.Instance.ActiveSaveGame.CharacterExperienceState.ActiveCharacterId);
		SetUnlockedBuildingStates();
	}

	private void RegisterGameControllerInitialized()
	{
		SingletonController<GameController>.Instance.Player.ResetVisuals();
		_gameControllerInitialized = true;
	}

	private void RegisterTutorialControllerInitialized()
	{
		_tutorialControllerInitialized = true;
	}

	private IEnumerator SpawnPlayerAfterReady()
	{
		bool tryToSpawn = true;
		while (tryToSpawn)
		{
			if (!SingletonController<GameController>.Instance.ShouldShowDemoPopup && _tutorialControllerInitialized && _gameControllerInitialized)
			{
				if (ShouldShowTownTutorialTutorial())
				{
					StartTownTutorial();
					tryToSpawn = false;
				}
				if (ShouldShowTitanicSoulTutorial())
				{
					yield return new WaitForSecondsRealtime(0.5f);
					StartTitanicSoulTutorial();
					tryToSpawn = false;
				}
				if (ShouldShowTalentTreeTutorial())
				{
					tryToSpawn = false;
				}
				if (!runningTutorial)
				{
					SpawnPlayerInTown();
					tryToSpawn = false;
				}
			}
			yield return new WaitForSecondsRealtime(0.2f);
		}
	}

	private bool ShouldShowTitanicSoulTutorial()
	{
		if (!SingletonController<TutorialController>.Instance.GetSaveState().ShownTitanicSoulTutorial && SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.TitanSouls) > 0)
		{
			return !runningTutorial;
		}
		return false;
	}

	private bool ShouldShowTalentTreeTutorial()
	{
		if (!SingletonController<TutorialController>.Instance.GetSaveState().ShownTalentTutorial && SingletonController<NewTalentController>.Instance.TalentPoints > 0)
		{
			return !runningTutorial;
		}
		return false;
	}

	private bool ShouldShowTownTutorialTutorial()
	{
		if (!SingletonController<TutorialController>.Instance.GetSaveState().ShownTownTutorial)
		{
			return !runningTutorial;
		}
		return false;
	}

	private void StartTitanicSoulTutorial()
	{
		SingletonController<SaveGameController>.Instance.ActiveSaveGame.TutorialSaveState.ShownTitanicSoulTutorial = true;
		runningTutorial = true;
		SpawnPlayerInTown();
		SingletonController<TutorialController>.Instance.ShowTitanicSoulTutorial(_player, _infrontOfUnlockAltarPosition, _tutorialCanvas, _gameplayCanvas, _insideUnlockAltarInteractionRangePosition, _titanicSoulsTutorialController, _titanSoulSprite);
	}

	private void StartTalentTreeTutorial()
	{
		runningTutorial = true;
		SpawnPlayerInTown();
		SingletonController<TutorialController>.Instance.ShowTalentTreeTutorial(_player);
	}

	private void StartTownTutorial()
	{
		runningTutorial = true;
		_player.transform.position = new Vector2(_spawnPointNewGame.position.x, _spawnPointNewGame.position.y);
		_cinemachineSwitcher.SwitchToPlayer();
		SingletonController<GameController>.Instance.Player.Spawn();
		SingletonController<TutorialController>.Instance.ShowTownTutorial(_player, this, _unlockableTownBuildings.FirstOrDefault((UnlockableInTown x) => x.Unlockable == Enums.Unlockable.UnlockAltar), _unlockableTownBuildings.FirstOrDefault((UnlockableInTown x) => x.Unlockable == Enums.Unlockable.AdventurePortal), _infrontOfGatePosition, _centerTownPosition, _infrontOfPortalPosition, _infrontOfUnlockAltarPosition, _fortressGate, _gameplayCanvas, _movementTip, _tutorialCanvas);
	}

	private void SpawnPlayerInTown()
	{
		if (SingletonController<GameController>.Instance.ExitingFromTrainingRoom)
		{
			_player.transform.position = new Vector2(_spawnPointTrainingRoom.position.x, _spawnPointTrainingRoom.position.y);
			SingletonController<GameController>.Instance.Player.Spawn(teleportIn: false);
		}
		else if (SingletonController<GameController>.Instance.ExitingFromPortal)
		{
			_player.transform.position = new Vector2(_spawnPointPortalRoom.position.x, _spawnPointPortalRoom.position.y);
			SingletonController<GameController>.Instance.Player.Spawn();
		}
		else
		{
			_player.transform.position = new Vector2(_spawnPointNewGame.position.x, _spawnPointContinueGame.position.y);
			SingletonController<GameController>.Instance.Player.Spawn(teleportIn: false);
		}
	}

	private void UnlocksController_OnUnlockableUnlocked(object sender, UnlockableUnlockedEventArgs e)
	{
		if (!e.FromLoad)
		{
			UnlockableInTown unlockableInTown = _unlockableTownBuildings.FirstOrDefault((UnlockableInTown x) => x.Unlockable == e.UnlockedItem.Unlockable);
			if (!(unlockableInTown == null))
			{
				unlockableInTown.UnlockAnimate(reopenUI: true, unlockableInTown.FocusOnElement);
			}
		}
	}

	public void SetUnlockedBuildingStates()
	{
		UnlockableInTown[] unlockableTownBuildings = _unlockableTownBuildings;
		foreach (UnlockableInTown unlockableInTown in unlockableTownBuildings)
		{
			if (SingletonController<UnlocksController>.Instance.IsUnlocked(unlockableInTown.Unlockable) && !CheckIfUnlockIsActiveCharacter(unlockableInTown))
			{
				unlockableInTown.UnlockStable();
			}
		}
	}

	private bool CheckIfUnlockIsActiveCharacter(UnlockableInTown unlockableTownBuilding)
	{
		bool num = SingletonController<CharactersController>.Instance.ActiveCharacter != null && unlockableTownBuilding.Unlockable == Enums.Unlockable.ExtraCharacter0 && SingletonController<CharactersController>.Instance.ActiveCharacter.Id == 1;
		bool flag = SingletonController<CharactersController>.Instance.ActiveCharacter != null && unlockableTownBuilding.Unlockable == Enums.Unlockable.ExtraCharacter1 && SingletonController<CharactersController>.Instance.ActiveCharacter.Id == 2;
		bool flag2 = SingletonController<CharactersController>.Instance.ActiveCharacter != null && unlockableTownBuilding.Unlockable == Enums.Unlockable.ExtraCharacter2 && SingletonController<CharactersController>.Instance.ActiveCharacter.Id == 3;
		bool flag3 = SingletonController<CharactersController>.Instance.ActiveCharacter != null && unlockableTownBuilding.Unlockable == Enums.Unlockable.ExtraCharacter3 && SingletonController<CharactersController>.Instance.ActiveCharacter.Id == 4;
		return num || flag || flag2 || flag3;
	}
}
