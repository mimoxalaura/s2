using System.Collections;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Shared.Interfaces;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.Game.Level;

internal class ShopLevelController : MonoBehaviour, IInitializable
{
	private LevelSO _levelSO;

	public bool IsInitialized { get; private set; }

	internal LevelSO LevelSO => _levelSO;

	private void Start()
	{
		SingletonController<EnemyController>.Instance.ResetActiveEnemyCount();
		LoadLevelSO();
		StartCoroutine(RunLevelStartupAnimations());
		IsInitialized = true;
		SingletonController<InputController>.Instance.SwitchToIngameActionMap();
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CharactersController>.Instance, RegisterCharactersControllerLoadedEvent);
	}

	private void LoadLevelSO()
	{
		_levelSO = SingletonController<GameDatabase>.Instance.GameDatabaseSO.ShopLevel;
	}

	private IEnumerator RunLevelStartupAnimations()
	{
		yield return new WaitForSeconds(0.5f);
		SingletonController<GameController>.Instance.Player.Spawn();
		SingletonCacheController.Instance.GetControllerByType<LevelMusicPlayer>().PlayLevelMusic();
		SingletonController<GameController>.Instance.CanOpenMenu = true;
		SingletonController<GameController>.Instance.CanPauseGame = true;
		SingletonController<GameController>.Instance.SetGamePaused(gamePaused: false);
		yield return new WaitForSeconds(1.5f);
		SingletonController<GameController>.Instance.Player.SetCanAct(canAct: true);
	}

	internal void RegisterCharactersControllerLoadedEvent()
	{
		SingletonController<GameController>.Instance.Player.LoadCharacter(SingletonController<CharactersController>.Instance.ActiveCharacter);
	}
}
