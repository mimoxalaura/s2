using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Assets.Game.Revive;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.Stats;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BackpackSurvivors.Game.Game;

public class GameController : SingletonController<GameController>
{
	[SerializeField]
	private GameObject _draggableBagPrefab;

	[SerializeField]
	private GameObject _draggableItemPrefab;

	[SerializeField]
	private GameObject _draggableWeaponPrefab;

	[SerializeField]
	private SceneReference _creditsScene;

	[SerializeField]
	private AudioClip _failedAudio;

	[SerializeField]
	private AudioClip _failedAmbienceAudio;

	[SerializeField]
	private AudioClip _failedAudioSweep;

	[SerializeField]
	private AudioClip _failedMusicClip;

	[SerializeField]
	private AudioClip _succesAudio;

	private BackpackSurvivors.Game.Player.Player _player;

	private TimeBasedLevelController _levelController;

	private LootDropContainer _lootDropContainer;

	private bool _initialClearSkipped;

	internal bool CanOpenMenu = true;

	internal bool CanPauseGame = true;

	public bool ShouldShowDemoPopup;

	public bool HasShownDemoPopup;

	internal bool ExitingFromTrainingRoom;

	internal bool ExitingFromPortal;

	internal GameState GameState { get; private set; }

	internal LootDropContainer LootDropContainer => _lootDropContainer;

	internal Vector2 PlayerPosition => GetPlayer().transform.position;

	public BackpackSurvivors.Game.Player.Player Player => GetPlayer();

	internal bool GamePaused { get; private set; }

	public bool IsShowingOneTimeRewards { get; internal set; }

	internal event Action<bool> OnPauseUpdated;

	private void Start()
	{
		RegisterEvents();
		GameState = new GameState(null);
		base.IsInitialized = true;
		CanPauseGame = true;
	}

	public void SetGamePaused(bool gamePaused, bool setTimeScale = true)
	{
		if (CanPauseGame)
		{
			GamePaused = gamePaused;
			this.OnPauseUpdated?.Invoke(gamePaused);
			if (setTimeScale || !gamePaused)
			{
				Time.timeScale = (gamePaused ? 0f : 1f);
			}
		}
	}

	public void StartAdventure(AdventureSO adventure)
	{
		GameState = new GameState(adventure);
		SingletonController<StatisticsController>.Instance.SetStartTime();
		GameState.StorePlayerHealth();
		GoToLevelScene(GameState.GetCurrentLevel());
	}

	internal void ClearControllersOfAllState()
	{
		if (!_initialClearSkipped)
		{
			_initialClearSkipped = true;
			return;
		}
		GameState.ClearState();
		foreach (IClearable item in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<IClearable>())
		{
			if (item is IInitializable { IsInitialized: not false })
			{
				item.Clear();
			}
		}
	}

	internal void ClearControllersOfAdventureState(bool switchChar = true)
	{
		GameState.ClearState();
		foreach (IClearable item in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive: true).OfType<IClearable>())
		{
			if (item is IInitializable { IsInitialized: not false })
			{
				item.ClearAdventure();
			}
		}
		GameState.FinishGame();
		if (switchChar && SingletonController<CharactersController>.Instance.ActiveCharacter != null)
		{
			SingletonController<CharactersController>.Instance.SwitchCharacter(SingletonController<CharactersController>.Instance.ActiveCharacter);
		}
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SceneChangeController>.Instance, RegisterLevelSceneLoaded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<RelicsController>.Instance, RegisterRelicAdded);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoaded);
	}

	private void RegisterSaveGameLoaded()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	public void RegisterLevelSceneLoaded()
	{
		SingletonController<SceneChangeController>.Instance.OnLevelSceneLoaded += SceneChangeController_OnLevelSceneLoaded;
	}

	public void RegisterRelicAdded()
	{
		SingletonController<RelicsController>.Instance.OnRelicAdded += RelicsController_OnRelicAdded;
	}

	public void PlayerDied(bool delayUI = true)
	{
		if (!GameState.IsFinished)
		{
			GetPlayer().SetCanAct(canAct: false);
			TimeBasedLevelController controllerByType = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
			controllerByType.StopCountdown();
			controllerByType.StopLevelActivity(levelWon: false);
			GameState.FinishGame();
			SingletonController<StatisticsController>.Instance.SetEndTime();
			SingletonController<AudioController>.Instance.PlayMusicClip(_failedMusicClip);
			SingletonController<AudioController>.Instance.PlaySFXClip(_failedAudioSweep, 2f);
			DestroyAllLevelAssets(killEnemies: false);
			StartCoroutine(ShowDeathUI(delayUI));
		}
	}

	public void PlayerDiedButHasRevives()
	{
		if (!GameState.IsFinished)
		{
			DestroyProjectileAssets();
			GetPlayer().StopAllMovement();
			GetPlayer().SetCanAct(canAct: false);
			SingletonController<ReviveController>.Instance.ShowReviveUI();
		}
	}

	public void DestroyAllLevelAssets(bool killEnemies = true)
	{
		DestroyProjectileAssets();
		TargetArrowController targetArrowController = UnityEngine.Object.FindAnyObjectByType<TargetArrowController>();
		if (targetArrowController != null)
		{
			targetArrowController.RemoveAllArrows();
		}
		if (killEnemies)
		{
			StartCoroutine(DestroyLevelEnemiesAsync());
		}
	}

	public void DestroyProjectileAssets()
	{
		StartCoroutine(DestroyLProjectileAssetsAsync());
	}

	private IEnumerator DestroyLProjectileAssetsAsync()
	{
		for (int i = 0; i < 5; i++)
		{
			WeaponAttack[] array = UnityEngine.Object.FindObjectsByType<WeaponAttack>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].ExecuteDestroy(destroyedByHit: true);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator DestroyLevelEnemiesAsync()
	{
		for (int i = 0; i < 2; i++)
		{
			Enemy[] array = UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Kill();
			}
			yield return new WaitForSeconds(0.25f);
		}
	}

	public void RefreshPlayer()
	{
		_player = null;
		GetPlayer();
	}

	private IEnumerator ShowDeathUI(bool delay = true)
	{
		Time.timeScale = 0.5f;
		yield return new WaitForSeconds(0.2f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_failedAudio, 1f);
		SingletonController<AudioController>.Instance.PlaySFXClip(_failedAmbienceAudio, 1f);
		Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
		foreach (Enemy obj in array)
		{
			obj.EnemyMovement.StopMovement();
			obj.EnemyMovement.PreventMovement();
			obj.Knockback(GetPlayer().Transform, 25f);
		}
		if (delay)
		{
			yield return new WaitForSeconds(2.8f);
		}
		SingletonController<InputController>.Instance.SwitchToUIActionMap();
		SingletonCacheController.Instance.GetControllerByType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.AdventureCompleted);
	}

	private void RelicsController_OnRelicAdded(object sender, RelicAddedEventArgs e)
	{
		if (e.ByUser)
		{
			NextLevel();
		}
	}

	public void NextLevel()
	{
		GameState.AdvanceLevel();
		LevelSO currentLevel = GameState.GetCurrentLevel();
		GoToLevelScene(currentLevel);
	}

	private void SceneChangeController_OnLevelSceneLoaded(object sender, EventArgs e)
	{
		_levelController = SingletonCacheController.Instance.GetControllerByType<TimeBasedLevelController>();
		if (_levelController != null)
		{
			_levelController.OnLevelCompleted += LevelController_OnLevelCompleted;
		}
		_lootDropContainer = UnityEngine.Object.FindObjectOfType<LootDropContainer>();
	}

	private void LevelController_OnLevelCompleted(object sender, EventArgs e)
	{
		_levelController.OnLevelCompleted -= LevelController_OnLevelCompleted;
		SingletonController<GameController>.Instance.Player.HealthSystem.HealComplete();
		StartCoroutine(LevelFinishedAnimation());
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		if (e.SaveGame.DemoSaveState != null)
		{
			HasShownDemoPopup = e.SaveGame.DemoSaveState.HasShownDemoPopup;
		}
	}

	public void MovePickupsToPlayer()
	{
		List<Enums.LootType> lootTypesToMove = new List<Enums.LootType>
		{
			Enums.LootType.Coins,
			Enums.LootType.Health,
			Enums.LootType.TitanicSouls
		};
		LootDropContainer.Instance.MoveLootdropsToPlayer(fromCompletion: true, lootTypesToMove);
	}

	public void ProgressGameState()
	{
		if (GameState.HasLevelRemaining())
		{
			SingletonController<RelicsController>.Instance.SetRelicSource(Enums.RelicSource.Miniboss);
			SingletonCacheController.Instance.GetControllerByType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.RelicReward);
			return;
		}
		SingletonController<InputController>.Instance.CanCancel = true;
		if (!GameState.IsFinished)
		{
			SingletonController<StatisticsController>.Instance.SetEndTime();
			GameState.FinishGame();
			SingletonController<AudioController>.Instance.PlayMusicClip(_succesAudio);
			SingletonController<AdventureCompletedController>.Instance.SetSuccess();
			UnlockAdventureWonAchievement();
			ShouldShowDemoPopup = !SingletonController<SaveGameController>.Instance.ActiveSaveGame.DemoSaveState.HasShownDemoPopup;
			UnityEngine.Object.FindAnyObjectByType<ModalUiController>().OpenModalUI(Enums.ModalUITypes.AdventureCompleted);
		}
	}

	private void UnlockAdventureWonAchievement()
	{
		if (GameDatabase.IsDemo)
		{
			SingletonController<SteamController>.Instance.UnlockAchievement(Enums.AchievementEnum.Demo_AdventureWon);
		}
	}

	private IEnumerator LevelFinishedAnimation()
	{
		SingletonController<InputController>.Instance.CanCancel = false;
		Player.ShowSuccesEffect();
		yield return new WaitForSecondsRealtime(4f);
		ProgressGameState();
	}

	private void GoToLevelScene(LevelSO levelSO)
	{
		SingletonController<SceneChangeController>.Instance.ChangeScene(levelSO.LevelScene.ScenePath, LoadSceneMode.Single, isLevelScene: true);
	}

	private BackpackSurvivors.Game.Player.Player GetPlayer()
	{
		if (_player == null)
		{
			_player = UnityEngine.Object.FindObjectOfType<BackpackSurvivors.Game.Player.Player>();
		}
		return _player;
	}

	public override void Clear()
	{
		HasShownDemoPopup = false;
		ShouldShowDemoPopup = false;
	}

	public override void ClearAdventure()
	{
	}

	internal void ClearSceneChangeStates()
	{
		ExitingFromTrainingRoom = false;
		ExitingFromPortal = false;
	}
}
