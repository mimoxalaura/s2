using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Saving;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Saving;

public class SaveGameController : SingletonController<SaveGameController>
{
	public delegate void SaveGameLoadedHandler(object sender, SaveGameLoadedArgs e);

	[SerializeField]
	private bool _DEBUGCreateFullyUnlockedSave;

	public List<SaveGame> SaveGames;

	public SaveGame ActiveSaveGame;

	public int BuildNumber;

	public event SaveGameLoadedHandler OnSaveGameLoaded;

	private void Start()
	{
		base.IsInitialized = true;
	}

	[Command("progression.save", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SaveProgression()
	{
		try
		{
			ActiveSaveGame.RefreshSaveGameState();
			SaveFileController.Save(ActiveSaveGame, SaveFileController.CreateAndReturnFullFilePath(ActiveSaveGame.key));
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void SaveProgression(Guid key)
	{
		ActiveSaveGame.RefreshSaveGameState();
		SaveFileController.Save(ActiveSaveGame, SaveFileController.CreateAndReturnFullFilePath(key));
	}

	public void LoadProgression()
	{
		LoadProgression(ActiveSaveGame.key);
	}

	public void DeleteProgression(Guid key)
	{
		SaveFileController.Delete(key);
		if (ActiveSaveGame != null && ActiveSaveGame.key == key)
		{
			ActiveSaveGame = null;
		}
	}

	[Command("progression.load", Platform.AllPlatforms, MonoTargetType.Single)]
	public void LoadProgression(string key)
	{
		LoadProgression(Guid.Parse(key));
	}

	public void LoadProgression(Guid key)
	{
		if (_DEBUGCreateFullyUnlockedSave)
		{
			DEBUG_CreateFullyUnlockedSave();
			return;
		}
		SaveGame progressionState = (ActiveSaveGame = SaveFileController.Load(key));
		ActiveSaveGame.Validate();
		this.OnSaveGameLoaded?.Invoke(this, new SaveGameLoadedArgs(progressionState));
	}

	private void DEBUG_CreateFullyUnlockedSave()
	{
		SaveGame saveGame = new SaveGame(Guid.Empty);
		foreach (Enums.Unlockable value in Enum.GetValues(typeof(Enums.Unlockable)))
		{
			saveGame.UnlockedUpgradesState.UnlockedUpgrades[value] = 1;
		}
		ActiveSaveGame = saveGame;
		this.OnSaveGameLoaded?.Invoke(this, new SaveGameLoadedArgs(saveGame));
	}

	public void SetActiveSaveGame(SaveGame saveGame)
	{
		ActiveSaveGame = saveGame;
	}

	public List<SaveGame> GetSaveGames()
	{
		return SaveFileController.LoadAllFiles();
	}

	public void RefreshSaveGames()
	{
		SaveGames = GetSaveGames();
	}

	public override void AfterBaseAwake()
	{
		base.AfterBaseAwake();
		RefreshSaveGames();
		if (SaveGames.Any((SaveGame x) => x.HasData()))
		{
			ActiveSaveGame = (from x in SaveGames
				where x.HasData()
				orderby x.StatisticsState.LastPlayed descending
				select x).First();
		}
	}

	public override void Clear()
	{
	}

	public override void ClearAdventure()
	{
	}
}
